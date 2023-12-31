﻿using System;
using UnityEngine;

namespace Assets.Src.Model
{
    public class VectorContainer
    {
        public Guid UnitId { get; private set; }
        public Vector3 OriginPoint { get; private set; }
        public Vector3 DestinationPoint { get; private set; }

        public void UpdateVectors(Vector3 newFirstVector, Vector3 newSecondVector)
        {
            OriginPoint = newFirstVector;
            DestinationPoint = newSecondVector;
        }
    }
}
