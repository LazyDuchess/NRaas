using Sims3.Gameplay;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Hooks
{
    /// <summary>
    /// Misc. functions to aid with hooking.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Attempts to replace the function call of a Simulator object, such as Alarms, OneShotFunctions, etc.
        /// </summary>
        /// <typeparam name="T">The expected Type of the Delegate.</typeparam>
        /// <param name="obj">Object to replace delegate of.</param>
        /// <param name="newDelegate">The new delegate to use.</param>
        /// <returns>Whether the function succeeded.</returns>
        public static bool SetDelegateForSimulatorObject<T>(object obj, T newDelegate) where T : Delegate
        {
            return SetDelegateForSimulatorObject(obj, newDelegate);
        }

        /// <summary>
        /// Attempts to replace the function call of a Simulator object, such as Alarms, OneShotFunctions, etc.
        /// </summary>
        /// <param name="obj">Object to replace delegate of.</param>
        /// <param name="newDelegate">The new delegate to use.</param>
        /// <returns>Whether the function succeeded.</returns>
        public static bool SetDelegateForSimulatorObject(object obj, Delegate newDelegate)
        {
            var functionField = GetFunctionFieldForSimulatorObject(obj);
            if (functionField == null)
                return false;
            try
            {
                functionField.SetValue(obj, newDelegate);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns Delegate for functions run in the Simulator, like Alarms, OneShotFunctions, etc.
        /// </summary>
        /// /// <typeparam name="T">The expected Type of the Delegate.</typeparam>
        /// <param name="obj">Object to return a Delegate for.</param>
        /// <returns>Delegate.</returns>
        public static T GetDelegateForSimulatorObject<T>(object obj) where T : Delegate
        {
            return GetDelegateForSimulatorObject(obj) as T;
        }

        /// <summary>
        /// Returns Delegate for functions run in the Simulator, like Alarms, OneShotFunctions, etc.
        /// </summary>
        /// <param name="obj">Object to return a Delegate for.</param>
        /// <returns>Delegate.</returns>
        public static Delegate GetDelegateForSimulatorObject(object obj)
        {
            // Might be slow but should catch all.
            var functionField = GetFunctionFieldForSimulatorObject(obj);
            if (functionField == null)
                return null;
            return (functionField.GetValue(obj) as Delegate);
        }

        static FieldInfo GetFunctionFieldForSimulatorObject(object obj)
        {
            var type = obj.GetType();
            var functionField = type.GetField("mFunction", BindingFlags.NonPublic | BindingFlags.Instance);
            if (functionField == null)
                functionField = type.GetField("mFunction", BindingFlags.Public | BindingFlags.Instance);
            return functionField;
        }

        /// <summary>
        /// Returns the full path for a method, including namespace and class (e.g. "NRaas.ErrorTrapSpace.Hooks.Helper.GetMethodFullName")
        /// </summary>
        /// <param name="method">Method to retrieve full path for.</param>
        /// <returns>Full path of method.</returns>
        public static string GetMethodFullName(MethodInfo method)
        {
            return method.DeclaringType.FullName + "." + method.Name;
        }
    }
}
