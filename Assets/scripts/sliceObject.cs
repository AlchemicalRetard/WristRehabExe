/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class sliceObject : MonoBehaviour
{
    public Transform plane;
    public GameObject targetOfSlice;
    public Material crossSectionMaterial;
    public Transform chopPoint;
    public float radius = 1f; // Max distance from the chopPoint
    private int sliceCount = 0; // Counter to track the number of slices made
    public int maxSlices = 7; // Maximum number of slices allowed

    public GameObject parentObject;
    public Animator animator;
    private bool canSlice = true; // Flag to control slicing availability

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && sliceCount < maxSlices && canSlice)
        {
            animator.SetBool("cut", true);
            StartCoroutine(Chop());
        }

        
    }

    IEnumerator Chop()
    {
        yield return new WaitForSeconds(1.5f);
       
        if (sliceCount < maxSlices) // Check again to ensure no additional slices are made during the wait
        {
            Slice(targetOfSlice);
            sliceCount++; // Increment the slice counter
        }
        if (sliceCount == maxSlices) // Once max slices reached, move remaining pieces
        {
            animator.SetTrigger("slide");
        }
    }

   *//* private void MovePiecesAside()
    {
        Vector3 moveDirection = new Vector3(0, 0,1f); // Move to the side along the X-axis
        foreach (Transform child in transform)
        {
            child.position += moveDirection; // Move each child piece to the side
        }
    }*//*

    public void Slice(GameObject target)
    {
        SlicedHull hull = target.Slice(plane.position, plane.up);

        if (hull != null)
        {
            Vector3 randomPos = Random.insideUnitSphere * radius;
            randomPos.y = 0;
            Vector3 spawnPos = chopPoint.position + randomPos;
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial,parentObject);

            if (upperHull != null && lowerHull != null)
            {
                targetOfSlice = upperHull; // Update targetOfSlice to be the new upper hull
                targetOfSlice.transform.position = upperHull.transform.position + new Vector3(0, 0, -0.050f);
                lowerHull.transform.position = spawnPos;
                animator.SetBool("cut", false);
                Destroy(target);
            }
        }
    }
   
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class sliceObject : MonoBehaviour
{
    public Transform plane;
    public GameObject targetOfSlice;
    public Material crossSectionMaterial;
    public Transform chopPoint;
    public float radius = 1f; // Max distance from the chopPoint
    public int sliceCount = 0; // Counter to track the number of slices made
    public int maxSlices = 7; // Maximum number of slices allowed

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip cuttingSound;

    public GameObject parentObject;
    public Animator animator;
    private bool canSlice = true; // Flag to control slicing availability

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && sliceCount < maxSlices && canSlice)
        {
            StartCoroutine(Chop());
        }
    }

    IEnumerator Chop()
    {
        canSlice = false; // Prevent further slicing actions
        animator.SetBool("cut", true);
        audioSource.PlayOneShot(cuttingSound);
        yield return new WaitForSeconds(1.5f); // Wait for initial delay

        if (sliceCount < maxSlices) // Check again to ensure no additional slices are made during the wait
        {
            Slice(targetOfSlice);
            sliceCount++; // Increment the slice counter
        }

       /* if (sliceCount == maxSlices) // Once max slices reached, move remaining pieces
        {
            animator.SetTrigger("slide");
        }*/

        animator.SetBool("cut", false);
        yield return new WaitForSeconds(.5f); // Wait for the slicing animation to complete  IMPORTANT

        canSlice = true; // Allow slicing again
    }
    public void SetNewSliceTarget(GameObject newTarget)
    {
        targetOfSlice = newTarget; // Set the new target
        sliceCount = 0; // Reset slice count for the new target
        canSlice = true; // Allow slicing on the new target
    }
    public void Slice(GameObject target)
    {
        SlicedHull hull = target.Slice(plane.position, plane.up);

        if (hull != null)
        {
            Vector3 randomPos = Random.insideUnitSphere * radius;
            randomPos.y = 0;
            Vector3 spawnPos = chopPoint.position + randomPos;
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial, parentObject);

            if (upperHull != null && lowerHull != null)
            {
                upperHull.tag = "MovableObject";
                lowerHull.tag = "MovableObject";
                targetOfSlice = upperHull; // Update targetOfSlice to be the new upper hull
                targetOfSlice.transform.position = upperHull.transform.position + new Vector3(0, 0, -0.050f);
                lowerHull.transform.position = spawnPos;
                Destroy(target);
            }
        }
    }
}
