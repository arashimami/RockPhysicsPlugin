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
    /// This class contains all the methods and subclasses of the Sc4AND5_DictionaryWellLogToDictionaryWorkspace.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class Sc4AND5_DictionaryWellLogToDictionaryWorkspace : Workstep<Sc4AND5_DictionaryWellLogToDictionaryWorkspace.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override Sc4AND5_DictionaryWellLogToDictionaryWorkspace.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
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
                return "5c16e815-43bc-4033-ad76-52f90431c692";
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
                var dictionaryWellLogVersion = CreateDictionaryWellLogVersionFromWellLog(originalDictionaryWellLog);
                //
                var newDictionaryWellLog = CreateDictionaryWellLog(originalDictionaryWellLog, multiplier, addend, dictionaryWellLogVersion);
                //
                // update output arguments
                arguments.ResultDictionaryWellLog = newDictionaryWellLog;
            }

            private DictionaryWellLog CreateDictionaryWellLog(DictionaryWellLog originalWellLog, double multiplier, double addend, DictionaryWellLogVersion wellLogVersion)
            {
                //
                //WellLog wellLog = WellLog.NullObject;
                DictionaryWellLog dictionaryWellLog = DictionaryWellLog.NullObject;
                //
                var borehole = originalWellLog.Borehole;
                //
                using (var transaction = DataManager.NewTransaction())
                {
                    transaction.Lock(borehole);
                    //
                    //wellLog = borehole.Logs.CreateWellLog(wellLogVersion);
                    dictionaryWellLog = borehole.Logs.CreateDictionaryWellLog(wellLogVersion);
                    //
                    //var wellLogSamples = new List<WellLogSample>();
                    var dictionaryWellLogSamples = new List<DictionaryWellLogSample>();
                    foreach (var wellLogSample in originalWellLog.Samples)
                    {
                        dictionaryWellLogSamples.Add(new DictionaryWellLogSample(wellLogSample.MD, (int)(multiplier * wellLogSample.Value + addend)));
                    }
                    //
                    dictionaryWellLog.Samples = dictionaryWellLogSamples.ToArray();
                    //
                    transaction.Commit();
                }
                //
                return dictionaryWellLog;
            }

            public static DictionaryWellLogVersion CreateDictionaryWellLogVersionFromWellLog(DictionaryWellLog dictionaryWellLog)
            {
                Slb.Ocean.Petrel.DomainObject.Well.DictionaryWellLogVersion dictionaryWellLogVersion = DictionaryWellLogVersion.NullObject;
                //
                Borehole borehole = dictionaryWellLog.Borehole;
                //
                DictionaryTemplate dictionaryWellLogTemplate = PetrelProject.WellKnownTemplates.FaciesGroup.Facies;
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
                        dictionaryWellLogVersion = logVersionCollection.CreateDictionaryWellLogVersion("Copied in " + "Sc#4AND5",
                            dictionaryWellLogTemplate);

                        //
                        transaction.Commit();
                    }
                }
                //
                //return wellLogVersion;
                return dictionaryWellLogVersion;
            }

        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for Sc4AND5_DictionaryWellLogToDictionaryWorkspace.
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
            private DictionaryWellLog resultDictionaryWellLog;

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


            [Description("ResultDictionaryWellLog", "Result Dictionary Well Log")]
            public DictionaryWellLog ResultDictionaryWellLog
            {
                get { return this.resultDictionaryWellLog; }
                internal set { this.resultDictionaryWellLog = value; }
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
        /// Gets the description of the Sc4AND5_DictionaryWellLogToDictionaryWorkspace
        /// </summary>
        public IDescription Description
        {
            get { return Sc4AND5_DictionaryWellLogToDictionaryWorkspaceDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the Sc4AND5_DictionaryWellLogToDictionaryWorkspace.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class Sc4AND5_DictionaryWellLogToDictionaryWorkspaceDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static Sc4AND5_DictionaryWellLogToDictionaryWorkspaceDescription instance = new Sc4AND5_DictionaryWellLogToDictionaryWorkspaceDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static Sc4AND5_DictionaryWellLogToDictionaryWorkspaceDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of Sc4AND5_DictionaryWellLogToDictionaryWorkspace
            /// </summary>
            public string Name
            {
                get { return "Sc4AND5_DictionaryWellLogToDictionaryWorkspace"; }
            }
            /// <summary>
            /// Gets the short description of Sc4AND5_DictionaryWellLogToDictionaryWorkspace
            /// </summary>
            public string ShortDescription
            {
                get { return "Input: DictionaryWellLog, Ouput: DictionaryWellLog"; }
            }
            /// <summary>
            /// Gets the detailed description of Sc4AND5_DictionaryWellLogToDictionaryWorkspace
            /// </summary>
            public string Description
            {
                get { return "We are working with descriptive logs in this scenario."; }
            }

            #endregion
        }
        #endregion


    }
}