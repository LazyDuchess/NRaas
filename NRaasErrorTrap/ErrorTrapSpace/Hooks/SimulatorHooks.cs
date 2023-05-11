using System;
using System.Collections.Generic;
using System.Text;
using ScriptCore;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.UI;

namespace NRaas.ErrorTrapSpace.Hooks
{
    /// <summary>
    /// Public API that interfaces with ScriptCore.Simulator. Use this to intercept calls to the Simulator.
    /// </summary>
    public class SimulatorHooks : Common.IPreLoad
    {
        /// <summary>
        /// Triggers when a script object is added to the Simulator. The passed arguments can be modified to replace any task or nullified to prevent its creation.
        /// </summary>
        public static AddObject OnAddObject;
        /// <summary>
        /// Triggers when an object finishes creation.
        /// </summary>
        public static CreatedObject OnCreatedObject;
        /// <summary>
        /// Triggers when an object is about to be destroyed.
        /// </summary>
        public static DestroyObject OnDestroyObject;

        public delegate void AddObject(ScriptCore.SimulatorHooks.AddObjectArgs args);
        public delegate void CreatedObject(ObjectGuid objectId);
        // Returning void here unlike ScriptCore.SimulatorHooks.DestroyObject because there is not much point to changing the Guid. Trying to prevent a deletion this way can cause issues.
        public delegate void DestroyObject(ObjectGuid objectId);

        public void OnPreLoad()
        {
            ScriptCore.SimulatorHooks.OnCreatedObject += ProcessOnCreatedObject;
            ScriptCore.SimulatorHooks.OnDestroyObject += ProcessOnDestroyObject;
            ScriptCore.SimulatorHooks.OnAddObject += ProcessOnAddObject;
        }

        void ProcessOnCreatedObject(ObjectGuid objectId)
        {
            OnCreatedObject?.Invoke(objectId);
        }

        ObjectGuid ProcessOnDestroyObject(ObjectGuid objectId)
        {
            OnDestroyObject?.Invoke(objectId);
            return objectId;
        }

        void ProcessOnAddObject(ScriptCore.SimulatorHooks.AddObjectArgs args)
        {
            OnAddObject?.Invoke(args);
        }
    }
}
