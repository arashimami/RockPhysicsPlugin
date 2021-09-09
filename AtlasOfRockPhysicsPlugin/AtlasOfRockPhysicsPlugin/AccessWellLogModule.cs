using System;
using Slb.Ocean.Core;
using Slb.Ocean.Petrel;
using Slb.Ocean.Petrel.UI;
using Slb.Ocean.Petrel.Workflow;

namespace AtlasOfRockPhysicsPlugin
{
    /// <summary>
    /// This class will control the lifecycle of the Module.
    /// The order of the methods are the same as the calling order.
    /// </summary>
    [ModuleAppearance(typeof(AccessWellLogModuleAppearance))]
    public class AccessWellLogModule : IModule
    {
        private Process m_sc7_modifywelllogworkstepInstance;
        private Process m_sc6_dictionarywelllogtowelllogworkspaceInstance;
        private Process m_Sc4AND5_DictionaryWellLogToDictionaryWorkspaceInstance;
        private Process m_sc2_copywelllogtootherwelllogformworkstepInstance;
        private Process m_sc3_welllogtodictionaryworkspaceInstance;
        private Process m_Sc1_CopyWellLogWithLinearTransformWorkstepInstance;
        public AccessWellLogModule()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region IModule Members

        /// <summary>
        /// This method runs once in the Module life; when it loaded into the petrel.
        /// This method called first.
        /// </summary>
        public void Initialize()
        {
            DataManager.WorkspaceEvents.Opened += this.WorkspaceOpened;
            DataManager.WorkspaceEvents.Closing += this.WorkspaceClosing;
            DataManager.WorkspaceEvents.Closed += this.WorkspaceClosed;

            // TODO:  Add AccessWellLogModule.Initialize implementation
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the not UI related components.
        /// (eg: datasource, plugin)
        /// </summary>
        public void Integrate()
        {
            // Register AtlasOfRockPhysicsPlugin.Sc7_ModifyWellLogWorkstep
            AtlasOfRockPhysicsPlugin.Sc7_ModifyWellLogWorkstep sc7_modifywelllogworkstepInstance = new AtlasOfRockPhysicsPlugin.Sc7_ModifyWellLogWorkstep();
            PetrelSystem.WorkflowEditor.Add(sc7_modifywelllogworkstepInstance);
            m_sc7_modifywelllogworkstepInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(sc7_modifywelllogworkstepInstance);
            PetrelSystem.ProcessDiagram.Add(m_sc7_modifywelllogworkstepInstance, "Plug-ins");
            // Register AtlasOfRockPhysicsPlugin.Sc6_DictionaryWellLogToWellLogWorkspace
            AtlasOfRockPhysicsPlugin.Sc6_DictionaryWellLogToWellLogWorkspace sc6_dictionarywelllogtowelllogworkspaceInstance = new AtlasOfRockPhysicsPlugin.Sc6_DictionaryWellLogToWellLogWorkspace();
            PetrelSystem.WorkflowEditor.Add(sc6_dictionarywelllogtowelllogworkspaceInstance);
            m_sc6_dictionarywelllogtowelllogworkspaceInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(sc6_dictionarywelllogtowelllogworkspaceInstance);
            PetrelSystem.ProcessDiagram.Add(m_sc6_dictionarywelllogtowelllogworkspaceInstance, "Plug-ins");
            // Register AtlasOfRockPhysicsPlugin.Sc4_DictionaryWellLogToDictionaryWorkspace
            AtlasOfRockPhysicsPlugin.Sc4AND5_DictionaryWellLogToDictionaryWorkspace Sc4AND5_DictionaryWellLogToDictionaryWorkspace = new AtlasOfRockPhysicsPlugin.Sc4AND5_DictionaryWellLogToDictionaryWorkspace();
            PetrelSystem.WorkflowEditor.Add(Sc4AND5_DictionaryWellLogToDictionaryWorkspace);
            m_Sc4AND5_DictionaryWellLogToDictionaryWorkspaceInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(Sc4AND5_DictionaryWellLogToDictionaryWorkspace);
            PetrelSystem.ProcessDiagram.Add(m_Sc4AND5_DictionaryWellLogToDictionaryWorkspaceInstance, "Plug-ins");
            // Register AtlasOfRockPhysicsPlugin.Sc2_CopyWellLogToOtherWellLogformWorkstep
            AtlasOfRockPhysicsPlugin.Sc2_CopyWellLogToOtherWellLogformWorkstep sc2_copywelllogtootherwelllogformworkstepInstance = new AtlasOfRockPhysicsPlugin.Sc2_CopyWellLogToOtherWellLogformWorkstep();
            PetrelSystem.WorkflowEditor.Add(sc2_copywelllogtootherwelllogformworkstepInstance);
            m_sc2_copywelllogtootherwelllogformworkstepInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(sc2_copywelllogtootherwelllogformworkstepInstance);
            PetrelSystem.ProcessDiagram.Add(m_sc2_copywelllogtootherwelllogformworkstepInstance, "Plug-ins");
            // Register AtlasOfRockPhysicsPlugin.Sc3_WellLogToDictionaryWorkspace
            AtlasOfRockPhysicsPlugin.Sc3_WellLogToDictionaryWorkspace sc3_welllogtodictionaryworkspaceInstance = new AtlasOfRockPhysicsPlugin.Sc3_WellLogToDictionaryWorkspace();
            PetrelSystem.WorkflowEditor.Add(sc3_welllogtodictionaryworkspaceInstance);
            m_sc3_welllogtodictionaryworkspaceInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(sc3_welllogtodictionaryworkspaceInstance);
            PetrelSystem.ProcessDiagram.Add(m_sc3_welllogtodictionaryworkspaceInstance, "Plug-ins");
            // Register AtlasOfRockPhysicsPlugin.CopyWellLogWithLinearTransformWorkstep
            AtlasOfRockPhysicsPlugin.Sc1_CopyWellLogWithLinearTransformWorkstep copywelllogwithlineartransformworkstepInstance = new AtlasOfRockPhysicsPlugin.Sc1_CopyWellLogWithLinearTransformWorkstep();
            PetrelSystem.WorkflowEditor.Add(copywelllogwithlineartransformworkstepInstance);
            m_Sc1_CopyWellLogWithLinearTransformWorkstepInstance = new Slb.Ocean.Petrel.Workflow.WorkstepProcessWrapper(copywelllogwithlineartransformworkstepInstance);
            PetrelSystem.ProcessDiagram.Add(m_Sc1_CopyWellLogWithLinearTransformWorkstepInstance, "Plug-ins");

            // TODO:  Add AccessWellLogModule.Integrate implementation
        }

        /// <summary>
        /// This method runs once in the Module life. 
        /// In this method, you can do registrations of the UI related components.
        /// (eg: settingspages, treeextensions)
        /// </summary>
        public void IntegratePresentation()
        {

            // TODO:  Add AccessWellLogModule.IntegratePresentation implementation
        }

        /// <summary>
        /// IModule interface does not define this method. 
        /// It is an eventhandler method, which is subscribed in the Initialize() method above,
        /// and is called every time when Petrel creates or loads a project.
        /// </summary>
        private void WorkspaceOpened(object sender, WorkspaceEventArgs args)
        {

            // TODO:  Add Workspace Opened eventhandler implementation
        }

        /// <summary>
        /// IModule interface does not define this method. 
        /// It is an eventhandler method, which is subscribed in the Initialize() method above,
        /// and is called every time before Petrel closes a project.
        /// </summary>
        private void WorkspaceClosing(object sender, WorkspaceCancelEventArgs args)
        {
            // TODO:  Add Workspace Closing eventhandler implementation
        }

        /// <summary>
        /// IModule interface does not define this method. 
        /// It is an eventhandler method, which is subscribed in the Initialize() method above,
        /// and is called every time after Petrel closed a project.
        /// </summary>
        private void WorkspaceClosed(object sender, WorkspaceEventArgs args)
        {
            // TODO:  Add Workspace Closed eventhandler implementation
        }

        /// <summary>
        /// This method runs once in the Module life.
        /// right before the module is unloaded. 
        /// It usually happens when the application is closing.
        /// </summary>
        public void Disintegrate()
        {
            PetrelSystem.ProcessDiagram.Remove(m_sc7_modifywelllogworkstepInstance);
            PetrelSystem.ProcessDiagram.Remove(m_sc6_dictionarywelllogtowelllogworkspaceInstance);
            PetrelSystem.ProcessDiagram.Remove(m_Sc4AND5_DictionaryWellLogToDictionaryWorkspaceInstance);
            PetrelSystem.ProcessDiagram.Remove(m_sc2_copywelllogtootherwelllogformworkstepInstance);
            PetrelSystem.ProcessDiagram.Remove(m_sc3_welllogtodictionaryworkspaceInstance);
            PetrelSystem.ProcessDiagram.Remove(m_Sc1_CopyWellLogWithLinearTransformWorkstepInstance);
            DataManager.WorkspaceEvents.Opened -= this.WorkspaceOpened;
            DataManager.WorkspaceEvents.Closing -= this.WorkspaceClosing;
            DataManager.WorkspaceEvents.Closed -= this.WorkspaceClosed;

            // TODO:  Add AccessWellLogModule.Disintegrate implementation
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO:  Add AccessWellLogModule.Dispose implementation
        }

        #endregion

    }

    #region ModuleAppearance Class

    /// <summary>
    /// Appearance (or branding) for a Slb.Ocean.Core.IModule.
    /// This is associated with a module using Slb.Ocean.Core.ModuleAppearanceAttribute.
    /// </summary>
    internal class AccessWellLogModuleAppearance : IModuleAppearance
    {
        /// <summary>
        /// Description of the module.
        /// </summary>
        public string Description
        {
            get { return "AccessWellLogModule"; }
        }

        /// <summary>
        /// Display name for the module.
        /// </summary>
        public string DisplayName
        {
            get { return "AccessWellLogModule"; }
        }

        /// <summary>
        /// Returns the name of a image resource.
        /// </summary>
        public string ImageResourceName
        {
            get { return null; }
        }

        /// <summary>
        /// A link to the publisher or null.
        /// </summary>
        public Uri ModuleUri
        {
            get { return null; }
        }
    }

    #endregion
}