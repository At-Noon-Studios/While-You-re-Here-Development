using System;
using System.Collections.Generic;
using component.gardening;
using component.making_tea;
using component;
using component.making_tea;
using component.test;
using ScriptableObjects.chores;
using UnityEngine;

namespace chore
{
    public enum ChoreStatus
    {
        Inactive,
        Active,
        Completed
    };

    public class Chore
    {
        public event Action<Chore> OnChoreCompleted;

        public string ChoreName;
        public readonly int ChoreID;
        public ChoreStatus ChoreStatus;
        private readonly List<ChoreComponent> _choreComponents = new List<ChoreComponent>();

        private int _componentsCompleted;

        private readonly Dictionary<ChoreComponent.ChoreComponentType, System.Func<SoChoreComponent, ChoreComponent>>
            _componentFactory
                = new Dictionary<ChoreComponent.ChoreComponentType, Func<SoChoreComponent, ChoreComponent>>()
                {
                    { ChoreComponent.ChoreComponentType.WateringCanPickedUp, CcWateringCanPickedUp.CreateFactory },
                    { ChoreComponent.ChoreComponentType.WateringCanFilled , CcWateringCanFilled.CreateFactory},
                    { ChoreComponent.ChoreComponentType.PlantWatered, CcPlantWatered.CreateFactory },
                    { ChoreComponent.ChoreComponentType.KettleFilled, CcKettleFilled.CreateFactory },
                    { ChoreComponent.ChoreComponentType.WaterBoiled, CcWaterBoiled.CreateFactory },
                    { ChoreComponent.ChoreComponentType.TeabagAdded, CcTeabagAdded.CreateFactory },
                    { ChoreComponent.ChoreComponentType.CupFilled, CcCupFilled.CreateFactory }
                };

        public Chore(string name, int id, List<SoChoreComponent> choreComponents)
        {
            ChoreName = name;
            ChoreID = id;
            ChoreStatus = ChoreStatus.Inactive;

            // Don't continue if no components are added to the list
            if (choreComponents.Count <= 0)
                Debug.Log($"No chore components found for stage {name}");

            foreach (SoChoreComponent choreComponent in choreComponents)
            {
                ChoreComponent ccTemp = null;

                if (_componentFactory.ContainsKey(choreComponent.choreType))
                    ccTemp = _componentFactory[choreComponent.choreType](choreComponent);

                if (ccTemp == null)
                {
                    Debug.LogError(
                        $"Failed to create ChoreComponent of type {choreComponent.choreType} in chore {name}");
                    continue;
                }

                _choreComponents.Add(ccTemp);

                // Subscribe to the component so that the Chore knows when the component has been completed.
                ccTemp.OnComponentCompleted += ComponentCompleted;
            }
        }

        ~Chore()
        {
            // Unsubscribe all components from the onComponentComplete event
            for (int i = _choreComponents.Count - 1; i >= 0; i--)
            {
                _choreComponents[i].OnComponentCompleted -= ComponentCompleted;
                _choreComponents[i] = null;
            }
        }

        private void ComponentCompleted(ChoreComponent choreComponent)
        {
            _componentsCompleted++;

            Debug.Log($"Chore {ChoreName}: Component '{choreComponent.ComponentName}' was completed");

            if (_componentsCompleted == _choreComponents.Count)
            {
                // Chore has been completed
                ChoreStatus = ChoreStatus.Completed;
                OnChoreCompleted?.Invoke(this);
            }
            else
            {
                // Enable next component
                if (_choreComponents.Count > _componentsCompleted)
                    _choreComponents[_componentsCompleted].EnableComponent();
            }
        }

        public void Activate()
        {
            if (_choreComponents.Count < 1)
            {
                Debug.LogWarning($"Chore {ChoreName} has no components to activate!");
                return;
            }

            Debug.Log($"Activating chore {ChoreName}, first component: {_choreComponents?[0]}");
            _choreComponents[0].EnableComponent();
            ChoreStatus = ChoreStatus.Active;
        }
    }
}
