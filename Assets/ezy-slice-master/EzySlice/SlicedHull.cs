using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EzySlice {

    /**
     * The final generated data structure from a slice operation. This provides easy access
     * to utility functions and the final Mesh data for each section of the HULL.
     */
    public sealed class SlicedHull {
        private Mesh upper_hull;
        private Mesh lower_hull;

        public SlicedHull(Mesh upperHull, Mesh lowerHull) {
            this.upper_hull = upperHull;
            this.lower_hull = lowerHull;
        }

        public GameObject CreateUpperHull(GameObject original) {
            return CreateUpperHull(original, null);
        }

        public GameObject CreateUpperHull(GameObject original, Material crossSectionMat, GameObject parent = null)
        {
            GameObject newObject = CreateUpperHull(); // This call is assumed to create the upper hull GameObject

            if (newObject != null)
            {
                // Copy transform properties from the original object
                newObject.transform.localPosition = original.transform.localPosition;
                newObject.transform.localRotation = original.transform.localRotation;
                newObject.transform.localScale = original.transform.localScale;

                // Copy or modify materials based on the presence of a custom cross-section material
                Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
                Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

                // Check if the mesh subMesh count matches to determine if the cross-section has been batched with the submeshes
                if (mesh.subMeshCount == upper_hull.subMeshCount)
                {
                    newObject.GetComponent<Renderer>().sharedMaterials = shared;
                }
                else
                {
                    // If a cross-section material was used, create a new array of materials including the cross-section material
                    Material[] newShared = new Material[shared.Length + 1];
                    System.Array.Copy(shared, newShared, shared.Length);
                    newShared[shared.Length] = crossSectionMat; // Add the cross-section material at the end
                    newObject.GetComponent<Renderer>().sharedMaterials = newShared;
                }

                // Set the parent of the newObject to the provided parent GameObject, if any
                if (parent != null)
                {
                    newObject.transform.SetParent(parent.transform, false); // 'false' to maintain local orientation relative to the new parent
                }
            }

            return newObject;
        }


        public GameObject CreateLowerHull(GameObject original) {
            return CreateLowerHull(original, null);
        }

        public GameObject CreateLowerHull(GameObject original, Material crossSectionMat, GameObject parent = null)
        {
            GameObject newObject = CreateLowerHull();

            if (newObject != null)
            {
                newObject.transform.localPosition = original.transform.localPosition;
                newObject.transform.localRotation = original.transform.localRotation;
                newObject.transform.localScale = original.transform.localScale;

                Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
                Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

                if (mesh.subMeshCount == lower_hull.subMeshCount)
                {
                    newObject.GetComponent<Renderer>().sharedMaterials = shared;
                }
                else
                {
                    Material[] newShared = new Material[shared.Length + 1];
                    System.Array.Copy(shared, newShared, shared.Length);
                    newShared[shared.Length] = crossSectionMat;
                    newObject.GetComponent<Renderer>().sharedMaterials = newShared;
                }

                // Set parent if provided
                if (parent != null)
                {
                    newObject.transform.SetParent(parent.transform, false);
                }
            }

            return newObject;
        }


        /**
         * Generate a new GameObject from the upper hull of the mesh
         * This function will return null if upper hull does not exist
         */
        public GameObject CreateUpperHull() {
            return CreateEmptyObject("Upper_Hull", upper_hull);
        }

        /**
         * Generate a new GameObject from the Lower hull of the mesh
         * This function will return null if lower hull does not exist
         */
        public GameObject CreateLowerHull() {
            return CreateEmptyObject("Lower_Hull", lower_hull);
        }

        public Mesh upperHull {
            get { return this.upper_hull; }
        }

        public Mesh lowerHull {
            get { return this.lower_hull; }
        }

        /**
         * Helper function which will create a new GameObject to be able to add
         * a new mesh for rendering and return.
         */
        private static GameObject CreateEmptyObject(string name, Mesh hull) {
            if (hull == null) {
                return null;
            }

            GameObject newObject = new GameObject(name);

            newObject.AddComponent<MeshRenderer>();
            MeshFilter filter = newObject.AddComponent<MeshFilter>();

            filter.mesh = hull;

            return newObject;
        }
    }
}