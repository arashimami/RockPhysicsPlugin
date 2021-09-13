using System;

using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;
using Slb.Ocean.Petrel.DomainObject.Well;
using System.Collections.Generic;

namespace AtlasOfRockPhysicsPlugin
{
    /// <summary>
    /// This class contains all the methods and subclasses of the Sc7_ModifyWellLogWorkstep.
    /// Worksteps are displayed in the workflow editor.
    /// </summary>
    class Sc7_ModifyWellLogWorkstep : Workstep<Sc7_ModifyWellLogWorkstep.Arguments>, IExecutorSource, IAppearance, IDescriptionSource
    {
        #region Overridden Workstep methods

        /// <summary>
        /// Creates an empty Argument instance
        /// </summary>
        /// <returns>New Argument instance.</returns>

        protected override Sc7_ModifyWellLogWorkstep.Arguments CreateArgumentPackageCore(IDataSourceManager dataSourceManager)
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
                return "7c917b7f-efaf-4dd2-902a-3eb3ec2730e5";
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
                // extract input data
                var originalWellLog = arguments.InputWellLog;

                // modify well log
                var updatedWelllog = mudifyWellLog(originalWellLog);
            }

            private WellLog mudifyWellLog(WellLog wellLog)
            {
                using (var transaction = DataManager.NewTransaction())
                {
                    transaction.Lock(wellLog);

                    var wellLogSamples = new List<WellLogSample>();
                    foreach (var wellLogSample in wellLog.Samples)
                    {
                        wellLogSamples.Add(new WellLogSample(wellLogSample.MD, (float)(2500 - wellLogSample.MD)));
                    }
                    //
                    wellLog.Samples = wellLogSamples.ToArray();
                    //
                    transaction.Commit();
                }
                //
                return wellLog;
            }
            
        }

        #endregion

        /// <summary>
        /// ArgumentPackage class for Sc7_ModifyWellLogWorkstep.
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

            [Description("InputWellLog", "Input Well Log")]
            public WellLog InputWellLog
            {
                internal get { return this.inputWellLog; }
                set { this.inputWellLog = value; }
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
        /// Gets the description of the Sc7_ModifyWellLogWorkstep
        /// </summary>
        public IDescription Description
        {
            get { return Sc7_ModifyWellLogWorkstepDescription.Instance; }
        }

        /// <summary>
        /// This singleton class contains the description of the Sc7_ModifyWellLogWorkstep.
        /// Contains Name, Shorter description and detailed description.
        /// </summary>
        public class Sc7_ModifyWellLogWorkstepDescription : IDescription
        {
            /// <summary>
            /// Contains the singleton instance.
            /// </summary>
            private static Sc7_ModifyWellLogWorkstepDescription instance = new Sc7_ModifyWellLogWorkstepDescription();
            /// <summary>
            /// Gets the singleton instance of this Description class
            /// </summary>
            public static Sc7_ModifyWellLogWorkstepDescription Instance
            {
                get { return instance; }
            }

            #region IDescription Members

            /// <summary>
            /// Gets the name of Sc7_ModifyWellLogWorkstep
            /// </summary>
            public string Name
            {
                get { return "Sc7_ModifyWellLogWorkstep"; }
            }
            /// <summary>
            /// Gets the short description of Sc7_ModifyWellLogWorkstep
            /// </summary>
            public string ShortDescription
            {
                get { return "modifies the input log"; }
            }
            /// <summary>
            /// Gets the detailed description of Sc7_ModifyWellLogWorkstep
            /// </summary>
            public string Description
            {
                get { return "A test for modify the input log instead of creating a new one. It will work just for WellLogs."; }
            }

            #endregion
        }
        #endregion


    }
}