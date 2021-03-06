﻿//Copyright (c) matteo
//Selector.cs - com.tratteo.gibframe

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GibFrame.Selectors
{
    public abstract class Selector : MonoBehaviour
    {
        [SerializeField] protected string selectableTag;
        protected ISelectable currentSelected = null;
        protected Collider currentCollider;

        private List<Predicate<Collider>> predicates = new List<Predicate<Collider>>();

        public GameObject CurrentSelected { get => currentCollider != null ? currentCollider.gameObject : null; }

        public bool Active { get; private set; } = true;

        public T CurrentAs<T>() where T : class => currentSelected as T;

        public void InjectPredicates(params Predicate<Collider>[] pred)
        {
            foreach (Predicate<Collider> p in pred)
            {
                predicates.Add(p);
            }
        }

        public void SetActive(bool state)
        {
            Active = state;
        }

        public void ResetSelection()
        {
            currentSelected?.OnDeselect();
            currentSelected = null;
            currentCollider = null;
        }

        protected void Select(Collider newCollider)
        {
            if (newCollider != null)
            {
                currentCollider = newCollider;
                ISelectable newSelectable = currentCollider.gameObject.GetComponent<ISelectable>();
                if (newSelectable != null && currentSelected != newSelectable)
                {
                    if (currentSelected != null)
                    {
                        currentSelected?.OnDeselect();
                    }
                    currentSelected = newSelectable;
                    currentSelected.OnSelect();
                }
            }
        }

        protected bool ColliderSatisfiesPredicates(Collider collider)
        {
            foreach (Predicate<Collider> Predicate in predicates)
            {
                if (!Predicate(collider))
                {
                    return false;
                }
            }
            return true;
        }
    }
}