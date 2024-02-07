/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectsToBowl : MonoBehaviour
{
    // Public fields to be assigned through the Unity Inspector
    public Transform bowlTransform; // The target location for moving objects
    public float scatterRadius = 0.5f; // The radius within which objects will be scattered around the bowl
    public Transform spawnPoint; // The spawn point for new food objects
    public List<GameObject> foodPrefabs; // The list of food prefab GameObjects to instantiate

    // Private fields for internal control
    private int currentPrefabIndex = 0; // Index to track the current prefab to instantiate
    public GameObject sliced; // The GameObject that has the sliceObject script attached
    private sliceObject sliceScript; // Reference to the sliceObject script for accessing its properties

    void Start()
    {
        // Initialize the sliceScript reference from the sliced GameObject
        if (sliced != null)
        {
            sliceScript = sliced.GetComponent<sliceObject>();
        }
    }

    void Update()
    {
        // Check for left arrow key press to start moving objects to the bowl
        if (Input.GetKeyDown(KeyCode.LeftArrow) && sliceScript != null && sliceScript.sliceCount == sliceScript.maxSlices)
        {
            sliceScript.animator.SetBool("slide", true);
            StartCoroutine(MoveObjectsSmoothly());
        }
        // Check for right arrow key press to instantiate the next food object
        // but only after all slices have been made (sliceCount equals maxSlices)
        else if (Input.GetKeyDown(KeyCode.RightArrow) && sliceScript != null && sliceScript.sliceCount == sliceScript.maxSlices)
        {
            InstantiateNextFoodObject();
        }
    }

    IEnumerator MoveObjectsSmoothly()
    {
        GameObject[] objectsToMove = GameObject.FindGameObjectsWithTag("MovableObject");
        float duration = 4.0f; // Set movement duration to 4 seconds to match the animation
        float elapsedTime = 0;
        sliceScript.animator.SetBool("slide", false);
        Vector3[] finalPositions = new Vector3[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
            randomOffset.y = 0; // Ensure the offset is only horizontal
            finalPositions[i] = bowlTransform.position + randomOffset;
        }

        while (elapsedTime < duration)
        {
            for (int i = 0; i < objectsToMove.Length; i++)
            {
                GameObject obj = objectsToMove[i];
                obj.transform.position = Vector3.Lerp(obj.transform.position, finalPositions[i], elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectsToMove[i].transform.position = finalPositions[i];
            objectsToMove[i].tag = "Untagged"; // Optionally, untag the moved objects
        }
    }


    void InstantiateNextFoodObject()
    {
        if (currentPrefabIndex < foodPrefabs.Count)
        {
            GameObject newFoodObject = Instantiate(foodPrefabs[currentPrefabIndex], spawnPoint.position, foodPrefabs[currentPrefabIndex].transform.rotation); // removed Quternion identity causing problems.
            currentPrefabIndex++; // Move to the next prefab in the list

            // Set this new food object as the new target for slicing
            if (sliceScript != null)
            {
                sliceScript.SetNewSliceTarget(newFoodObject);
            }
        }
        else
        {
            Debug.Log("All food objects have been instantiated.");
        }
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectsToBowl : MonoBehaviour
{
    public Transform bowlTransform;
    public float scatterRadius = 0.5f;
    public Transform spawnPoint;
    public List<GameObject> foodPrefabs;

    public AudioSource audioSource;
    public AudioClip winSound;

    private int currentPrefabIndex = 0;
    public GameObject sliced;
    private sliceObject sliceScript;
    private bool isMovingObjects = false; // Flag to track if objects are currently being moved
    public GameObject CanvasObject;
    void Start()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (sliced != null)
        {
            sliceScript = sliced.GetComponent<sliceObject>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && sliceScript != null && sliceScript.sliceCount == sliceScript.maxSlices && !isMovingObjects)
        {
            isMovingObjects = true; // Set flag to true to indicate movement is in progress
            StartCoroutine(MoveObjectsSmoothly());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && sliceScript != null && sliceScript.sliceCount == sliceScript.maxSlices && !isMovingObjects)
        {
            // Only allow instantiation of the next food object if no objects are being moved
            InstantiateNextFoodObject();
        }
    }

    IEnumerator MoveObjectsSmoothly()
    {
        sliceScript.animator.SetBool("slide", true); // Start the slide animation

        GameObject[] objectsToMove = GameObject.FindGameObjectsWithTag("MovableObject");
        float duration = 2.0f; // Duration to match the animation
        float elapsedTime = 0;

        Vector3[] finalPositions = new Vector3[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * scatterRadius;
            randomOffset.y = 0;
            finalPositions[i] = bowlTransform.position + randomOffset;
        }

        while (elapsedTime < duration)
        {
            for (int i = 0; i < objectsToMove.Length; i++)
            {
                GameObject obj = objectsToMove[i];
                obj.transform.position = Vector3.Lerp(obj.transform.position, finalPositions[i], elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            objectsToMove[i].transform.position = finalPositions[i];
            objectsToMove[i].tag = "Untagged"; // Clean up tags if needed
        }

        
        isMovingObjects = false; // Reset the flag to allow new operations
    }

    void InstantiateNextFoodObject()
    {
        sliceScript.animator.SetBool("slide", false); // Ensure to reset the animation state if it's a toggle
        if (currentPrefabIndex < foodPrefabs.Count)
        {
            GameObject newFoodObject = Instantiate(foodPrefabs[currentPrefabIndex], spawnPoint.position, foodPrefabs[currentPrefabIndex].transform.rotation);
            currentPrefabIndex++;

            if (sliceScript != null)
            {
                sliceScript.SetNewSliceTarget(newFoodObject);
            }
        }
        else
        {
            CanvasObject.SetActive(true);
            audioSource.PlayOneShot(winSound);
            Debug.Log("All food objects have been instantiated.");
            Time.timeScale = 0;
        }
    }
}
