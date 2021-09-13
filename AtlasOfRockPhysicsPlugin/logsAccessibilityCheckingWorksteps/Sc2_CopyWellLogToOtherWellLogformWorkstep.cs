using System;

using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject.Well;
using System.Collections.Generic;
using Slb.Ocean.Petrel.DomainObject;

namespace AtlasOfRockPhysicsPlugin
{
    /// <summary>
    /// This class contains all the methods and subclasses of the Sc2_CopyWellLogToOtherWellLogformWorkstep.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class Sc2_CopyWellLogToOtherWellLogformWorkstep : Workstep<Sc2_CopyWellLogToOtherWellLogformWorkstep.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override Sc2_CopyWellLogToOtherWellLogformWorkstep.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
        {
            return new Arguments(dataSourceManager);
        }
        /// <summary>
        /// Copies the Arguments instance.
        /// </summary>
        /// <param name="fromArgumentPackage">the source Arguments instance</param>
        /// <param name="toArgumentPackage">the target Arguments instance</param>
        protected override void CopyArgumentPackageCore(Arguments fromArgumentPackage, Arguments toArgumentPackage)
        {
            DescribedArgumentsHelper.Copy(fromArgumentPackage, toArgumentPackage);
        }

        /// <summary>
        /// Gets the unique identifier for this Workstep.
        /// </summary>
        protected override string UniqueIdCore
        {
            get
            {
                return "ae732778-0467-4f77-bfd4-3b0192306e07";
            }
        }
        #endregion

        #region IExecutorSource Members and Executor class

        /// <summary>
        /// Creates the Executor instance for this workstep. This class will do the work of the Workstep.
        /// </summary>
        /// <param name="argumentPackage">the argumentpackage to pass to the Executor</param>
        /// <param name="workflowRuntimeContext">the context to pass to the Executor</param>
        /// <returns>The Executor instance.</returns>
        public Slb.Ocean.Petrel.Workflow.Executor GetExecutor(object argumentPackage, WorkflowRuntimeContext workflowRuntimeContext)
        {
            return new Executor(argumentPackage as Arguments, workflowRuntimeContext);
        }

        public class Executor : Slb.Ocean.Petrel.Workflow.Executor
        {
            Arguments arguments;
            WorkflowRuntimeContext context;

            public Executor(Arguments arguments, WorkflowRuntimeContext context)
            {
                this.arguments = arguments;
                this.context = context;
            }

            public override void ExecuteSimple()
            {
                // TODO: Implement the workstep logic here.
                // extract input data
                var originalWellLog = arguments.InputWellLog;
                var multiplier = arguments.Multiplier;
                var addend = arguments.Addend;

                //
                // create a well log version for the new well log 
                var wellLogVersion = CreateWellLogVersionFromWellLog(originalWellLog);
                //
                var newWellLog = CreateWellLog(originalWellLog, multiplier, addend, wellLogVersion);
                //
                // update output arguments
                arguments.ResultWellLog = newWellLog;
            }


            private WellLog CreateWellLog(WellLog originalWellLog, double multiplier, double addend, WellLogVersion wellLogVersion)
            {
                //
                WellLog wellLog = WellLog.NullObject;
                //
                var borehole = originalWellLog.Borehole;
                //
                using (var transaction = DataManager.NewTransaction())
                {
                    transaction.Lock(borehole);
                    //
                    wellLog = borehole.Logs.CreateWellLog(wellLogVersion);
                    //
                    var wellLogSamples = new List<WellLogSample>();
                    foreach (var wellLogSample in originalWellLog.Samples)
                    {
                        wellLogSamples.Add(new WellLogSample(wellLogSample.MD, (float)((multiplier * wellLogSample.Value + addend) / Math.Pow(10, 17))));
                    }
                    //
                    wellLog.Samples = wellLogSamples.ToArray();
                    //
                    transaction.Commit();
                }
                //
                return wellLog;
            }

            public static WellLogVersion CreateWellLogVersionFromWellLog(WellLog wellLog)
            {
                Slb.Ocean.Petrel.DomainObject.Well.WellLogVersion wellLogVersion = WellLogVersion.NullObject;
                //
                Borehole borehole = wellLog.Borehole;
                //
                Template wellLogTemplate = PetrelProject.WellKnownTemplates.PetrophysicalGroup.Permeability;
                //
                WellRoot wellRoot = WellRoot.Get(PetrelProject.PrimaryProject);
                //
                var logVersionCollection = wellRoot.LogVersionCollection;
                //
                if (!borehole.Logs.CanCreateWellLog(wellLog.WellLogVersion))
                {
                    using (var transaction = DataManager.NewTransaction())
                    {
                        transaction.Lock(logVersionCollection);
                        wellLogVersion = logVersionCollection.CreateWellLogVersion("Copied in " + "Sc#2",
                            wellLogTemplate);

                        //
                        transaction.Commit();
                    }
                }
                //
                return wellLogVersion;
            }
        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for Sc2_CopyWellLogToOtherWellLogformWorkstep.
        /// Each public property is an argument in the package.  The name, type and
        /// input/output role are taken from the property and modified by any
        /// attributes applied.
        /// </summary>
        public class Arguments : DescribedArgumentsByReflection
        {
            public Arguments()
                : this(DataManager.DataSourceManager)
            {                
            }

            public Arguments(IDataSourceManager dataSourceManager)
            {
            }


            private WellLog inputWellLog;
            private double multiplier = 1.0;
            private double addend = 0.0;
            private WellLog resultWellLog;

            [Description("InputWellLog", "Input Well Log")]
            public WellLog InputWellLog
            {
                internal get { return this.inputWellLog; }
                set { this.inputWellLog = value; }
            }

            public double Multiplier
            {
                internal get { return this.multiplier; }
                set { this.multiplier = value; }
            }


            public double Addend
            {
                internal get { return this.addend; }
                set { this.addend = value; }
            }


            [Description("ResultWellLog", "Result Well Log")]
            public WellLog ResultWellLog
            {
                get { return this.resultWellLog; }
                internal set { this.resultWellLog = value; }
            }
            
        }

        #region IAppearance Members
        public event EventHandler<TextChangedEventArgs> TextChanged;
        protected void RaiseTextChanged()
        {
            this.TextChanged?.Invoke(this, new TextChangedEventArgs(this));
        }

        public string Text
        {
            get { return Description.Name; }
            private set 
            {
                // TODO: implement set
                this.RaiseTextChanged();
            }
        }

        public event EventHandler<ImageChangedEventArgs> ImageChanged;
        protected void RaiseImageChanged()
        {
            this.ImageChanged?.Invoke(this, new ImageChangedEventArgs(this));
        }

        public System.Drawing.Bitmap Image
        {
            get { return PetrelImages.Modules; }
            private set 
            {
                // TODO: implement set
                this.RaiseImageChanged();
            }
        }
        #endregion

        #region IDescriptionSource Members

        /// <summary>
        /// Gets the description of the Sc2_CopyWellLogToOtherWellLogformWorkstep
        /// </summary>
        public IDescription Description
        {
            get { return Sc2_CopyWellLogToOtherWellLogformWorkstepDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the Sc2_CopyWellLogToOtherWellLogformWorkstep.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class Sc2_CopyWellLogToOtherWellLogformWorkstepDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static Sc2_CopyWellLogToOtherWellLogformWorkstepDescription instance = new Sc2_CopyWellLogToOtherWellLogformWorkstepDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static Sc2_CopyWellLogToOtherWellLogformWorkstepDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of Sc2_CopyWellLogToOtherWellLogformWorkstep
            /// </summary>
            public string Name
            {
                get { return "Sc2_CopyWellLogToOtherWellLogformWorkstep"; }
            }
            /// <summary>
            /// Gets the short description of Sc2_CopyWellLogToOtherWellLogformWorkstep
            /// </summary>
            public string ShortDescription
            {
                get { return "Input: WellLog, Output: WellLog but the template of the WellLog is changed"; }
            }
            /// <summary>
            /// Gets the detailed description of Sc2_CopyWellLogToOtherWellLogformWorkstep
            /// </summary>
            public string Description
            {
                get { return ""; }
            }

            #endregion
        }
        #endregion


    }
}