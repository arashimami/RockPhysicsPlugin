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
    /// This class contains all the methods and subclasses of the Sc3_WellLogToDictionaryWorkspace.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class Sc3_WellLogToDictionaryWorkspace : Workstep<Sc3_WellLogToDictionaryWorkspace.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override Sc3_WellLogToDictionaryWorkspace.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
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
                return "2ba6c8d1-1cdf-4ad0-80d1-491755db0bf1";
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
                var dictionaryWellLogVersion = CreateDictionaryWellLogVersionFromWellLog(originalWellLog);
                //
                var newDictionaryWellLog = CreateDictionaryWellLog(originalWellLog, multiplier, addend, dictionaryWellLogVersion);
                //
                // update output arguments
                arguments.ResultDictionaryWellLog = newDictionaryWellLog;
            }

            private DictionaryWellLog CreateDictionaryWellLog(WellLog originalWellLog, double multiplier, double addend, DictionaryWellLogVersion wellLogVersion)
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
                    dictionaryWellLog = borehole.Logs.CreateDictionaryWellLog(wellLogVersion);
                    //
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

            public static DictionaryWellLogVersion CreateDictionaryWellLogVersionFromWellLog(WellLog wellLog)
            {
                Slb.Ocean.Petrel.DomainObject.Well.DictionaryWellLogVersion dictionaryWellLogVersion = DictionaryWellLogVersion.NullObject;
                //
                Borehole borehole = wellLog.Borehole;
                //
                DictionaryTemplate dictionaryWellLogTemplate = PetrelProject.WellKnownTemplates.FaciesGroup.Facies;
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
                        dictionaryWellLogVersion = logVersionCollection.CreateDictionaryWellLogVersion("Copied in " + "Sc#3",
                            dictionaryWellLogTemplate);

                        //
                        transaction.Commit();
                    }
                }
                //
                return dictionaryWellLogVersion;
            }

        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for Sc3_WellLogToDictionaryWorkspace.
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
            private DictionaryWellLog resultDictionaryWellLog;

            [Description("InputWellLog", "Input Well Log")]
            public Slb.Ocean.Petrel.DomainObject.Well.WellLog InputWellLog
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
        /// Gets the description of the Sc3_WellLogToDictionaryWorkspace
        /// </summary>
        public IDescription Description
        {
            get { return Sc3_WellLogToDictionaryWorkspaceDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the Sc3_WellLogToDictionaryWorkspace.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class Sc3_WellLogToDictionaryWorkspaceDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static Sc3_WellLogToDictionaryWorkspaceDescription instance = new Sc3_WellLogToDictionaryWorkspaceDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static Sc3_WellLogToDictionaryWorkspaceDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of Sc3_WellLogToDictionaryWorkspace
            /// </summary>
            public string Name
            {
                get { return "Sc3_WellLogToDictionaryWorkspace"; }
            }
            /// <summary>
            /// Gets the short description of Sc3_WellLogToDictionaryWorkspace
            /// </summary>
            public string ShortDescription
            {
                get { return "Input: WellLog, Output: DictionaryWellLog"; }
            }
            /// <summary>
            /// Gets the detailed description of Sc3_WellLogToDictionaryWorkspace
            /// </summary>
            public string Description
            {
                get { return "The output well log can be changed to descriptive type using this workspace."; }
            }

            #endregion
        }
        #endregion


    }
}