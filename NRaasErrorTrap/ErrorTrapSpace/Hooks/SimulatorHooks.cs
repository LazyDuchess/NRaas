using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ScriptCore;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.UI;

using AddObjectArgs = ScriptCore.SimulatorHooks.AddObjectArgs;

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

        // Delegates for hooks.
        public delegate void AddObject(AddObjectArgs args);
        public delegate void CreatedObject(ObjectGuid objectId);
        // Returning void here unlike ScriptCore.SimulatorHooks.DestroyObject because there is not much point to changing the Guid. Trying to prevent a deletion this way can cause issues.
        public delegate void DestroyObject(ObjectGuid objectId);

        // Delegates for helpers.
        public delegate void AddFunction(AddObjectArgs args, Delegate function);
        public delegate void CreatedObjectOfType<T>(T createdObject) where T : class;
        public delegate void DestroyedObjectOfType<T>(T destroyedObject) where T : class;

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

        void ProcessOnAddObject(AddObjectArgs args)
        {
            OnAddObject?.Invoke(args);
        }

        public static AddObject AddOnFunctionAddByMethodInfo(MethodInfo methodInfo, AddFunction callback)
        {
            var callbackWrapper = new AddObject((AddObjectArgs args) =>
            {
                var dlg = Helper.GetDelegateForSimulatorObject(args.Object);
                if (dlg == null)
                    return;
                if (dlg.Method != methodInfo)
                    return;
                callback(args, dlg);
            });
            OnAddObject += callbackWrapper;
            return callbackWrapper;
        }

        public static AddObject AddOnFunctionAddByMethodName(string methodName, AddFunction callback)
        {
            var callbackWrapper = new AddObject((AddObjectArgs args) =>
            {
                var dlg = Helper.GetDelegateForSimulatorObject(args.Object);
                if (dlg == null)
                    return;
                if (dlg.Method.Name != methodName)
                    return;
                callback(args, dlg);
            });
            OnAddObject += callbackWrapper;
            return callbackWrapper;
        }

        public static AddObject AddOnFunctionAddByMethodFullName(string methodFullName, AddFunction callback)
        {
            var callbackWrapper = new AddObject((AddObjectArgs args) =>
            {
                var dlg = Helper.GetDelegateForSimulatorObject(args.Object);
                if (dlg == null)
                    return;
                if (Helper.GetMethodFullName(dlg.Method) != methodFullName)
                    return;
                callback(args, dlg);
            });
            OnAddObject += callbackWrapper;
            return callbackWrapper;
        }

        /// <summary>
        /// Adds a callback to Simulator.DestroyObject that listens for the destruction of an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="callback">Function to run when this object type gets destroyed.</param>
        /// <returns>Callback added to OnDestroyObject.</returns>
        public static DestroyObject AddOnObjectOfTypeDestroyedCallback<T>(DestroyedObjectOfType<T> callback) where T : class
        {
            var callbackWrapper = new DestroyObject((ObjectGuid target) =>
            {
                var targetObject = target.ObjectFromId<T>();
                if (targetObject == null)
                    return;
                callback(targetObject);
            });
            OnDestroyObject += callbackWrapper;
            return callbackWrapper;
        }

        /// <summary>
        /// Adds a callback to Simulator.CreateObject that listens for the creation of an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="callback">Function to run when this object type gets created.</param>
        /// <returns>Callback added to OnCreatedObject.</returns>
        public static CreatedObject AddOnObjectOfTypeCreatedCallback<T>(CreatedObjectOfType<T> callback) where T : class
        {
            var callbackWrapper = new CreatedObject((ObjectGuid target) =>
            {
                var targetObject = target.ObjectFromId<T>();
                if (targetObject == null)
                    return;
                callback(targetObject);
            });
            OnCreatedObject += callbackWrapper;
            return callbackWrapper;
        }
    }
}
