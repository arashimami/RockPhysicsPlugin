using System;

using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject.Well;
using Slb.Ocean.Petrel.DomainObject;
using System.Collections.Generic;

namespace AtlasOfRockPhysicsPlugin
{
    /// <summary>
    /// This class contains all the methods and subclasses of the Sc6_DictionaryWellLogToWellLogWorkspace.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class Sc6_DictionaryWellLogToWellLogWorkspace : Workstep<Sc6_DictionaryWellLogToWellLogWorkspace.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override Sc6_DictionaryWellLogToWellLogWorkspace.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
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
                return "4f757f84-b5f7-4672-b796-8ea5dccffa53";
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
                var originalDictionaryWellLog = arguments.InputDictionaryWellLog;
                var multiplier = arguments.Multiplier;
                var addend = arguments.Addend;

                //
                // create a well log version for the new well log 
                var wellLogVersion = CreateWellLogVersionFromWellLog(originalDictionaryWellLog);
                //
                var newWellLog = CreateWellLog(originalDictionaryWellLog, multiplier, addend, wellLogVersion);
                //
                // update output arguments
                arguments.ResultWellLog = newWellLog;
            }

            private WellLog CreateWellLog(DictionaryWellLog originalWellLog, double multiplier, double addend, WellLogVersion wellLogVersion)
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
                        wellLogSamples.Add(new WellLogSample(wellLogSample.MD, (float)((wellLogSample.Value + 2500 - wellLogSample.MD)/Math.Pow(10,6))));
                    }
                    //
                    wellLog.Samples = wellLogSamples.ToArray();
                    //
                    transaction.Commit();
                }
                //
                return wellLog;
            }

            public static WellLogVersion CreateWellLogVersionFromWellLog(DictionaryWellLog dictionaryWellLog)
            {
                Slb.Ocean.Petrel.DomainObject.Well.WellLogVersion wellLogVersion = WellLogVersion.NullObject;
                //
                Borehole borehole = dictionaryWellLog.Borehole;
                //
                Template wellLogTemplate = PetrelProject.WellKnownTemplates.PetrophysicalGroup.Porosity;
                //
                WellRoot wellRoot = WellRoot.Get(PetrelProject.PrimaryProject);
                //
                var logVersionCollection = wellRoot.LogVersionCollection;
                //
                if (!borehole.Logs.CanCreateDictionaryWellLog(dictionaryWellLog.DictionaryWellLogVersion))
                {
                    using (var transaction = DataManager.NewTransaction())
                    {
                        transaction.Lock(logVersionCollection);
                        wellLogVersion = logVersionCollection.CreateWellLogVersion("Copied in " + "Sc#6",
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
        /// ArgumentPackage class for Sc6_DictionaryWellLogToWellLogWorkspace.
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


            private DictionaryWellLog inputDictionaryWellLog;
            private double multiplier = 1.0;
            private double addend = 0.0;
            private WellLog resultWellLog;

            [Description("InputDictionaryWellLog", "Input Dictionary Well Log")]
            public DictionaryWellLog InputDictionaryWellLog
            {
                internal get { return this.inputDictionaryWellLog; }
                set { this.inputDictionaryWellLog = value; }
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
        /// Gets the description of the Sc6_DictionaryWellLogToWellLogWorkspace
        /// </summary>
        public IDescription Description
        {
            get { return Sc6_DictionaryWellLogToWellLogWorkspaceDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the Sc6_DictionaryWellLogToWellLogWorkspace.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class Sc6_DictionaryWellLogToWellLogWorkspaceDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static Sc6_DictionaryWellLogToWellLogWorkspaceDescription instance = new Sc6_DictionaryWellLogToWellLogWorkspaceDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static Sc6_DictionaryWellLogToWellLogWorkspaceDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of Sc6_DictionaryWellLogToWellLogWorkspace
            /// </summary>
            public string Name
            {
                get { return "Sc6_DictionaryWellLogToWellLogWorkspace"; }
            }
            /// <summary>
            /// Gets the short description of Sc6_DictionaryWellLogToWellLogWorkspace
            /// </summary>
            public string ShortDescription
            {
                get { return "Output well log will be in a numerical type, while the input well has been in a descriptive type."; }
            }
            /// <summary>
            /// Gets the detailed description of Sc6_DictionaryWellLogToWellLogWorkspace
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