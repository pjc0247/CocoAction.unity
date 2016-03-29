using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CocoAction
{
    public static class GameObjectExt
    {
		/// <summary>
        /// Executes an action.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action">an action object to execute</param>
        /// <returns>the <paramref name="action"/> object which you passed.</returns>
        public static Action RunAction(this GameObject target, Action action)
        {
            ActionManager.instance.AddAction(action);
            action.Start(target);

            return action;
        }
    }
    public class ActionManager : MonoBehaviour
    {
        private static ActionManager _instance;
        public static ActionManager instance {
            get
            {
                if (_instance == null)
                    Initialize();

                return _instance;
            }
        }

        private List<Action> actions { get; set; }
        
        public int runningActions
        {
            get
            {
                return actions.Count;
            }
        }

        private static void Initialize()
        {
            if (_instance != null)
                return;

            var obj = new GameObject("ActionManager");
            _instance = obj.AddComponent<ActionManager>();
        }

        ActionManager()
        {
            actions = new List<Action>();
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
        }
        public void RemoveAction(Action action)
        {
            actions.Remove(action);
        }

        void Update()
        {
            var trashBin = new List<Action>();

            foreach (var action in actions)
            {
                action.Update();

                if (action.isDone)
                    trashBin.Add(action);
            }

            foreach (var action in trashBin)
                RemoveAction(action);
        }
    }
}
