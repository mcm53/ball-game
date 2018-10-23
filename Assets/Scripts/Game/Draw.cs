using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour {

	public GameObject linePrefab;
	private GameObject line;
	private GameObject previousLine;
	private List<GameObject> previousLineColliders = new List<GameObject>();
	private Vector3 previousMouseCoordinates;
	private Vector3 previousMousePosition;
	private bool isDrawing = false;
	private int vectorCount = 0;
	private List<GameObject> lines = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			isDrawing = true;

			previousLineColliders.Clear();

			// Check that surface is drawable
			var hit = GetHit();
			if (hit.collider == null || !hit.collider.CompareTag("Drawable")) {
				return;
			}
			var mousePosition = GetMousePosition(hit);
			
			// Create new line
			var newLine = Instantiate(linePrefab, mousePosition, Quaternion.identity);
			lines.Add(newLine);
			line = newLine;

			// Set first point
			var lineRenderer = GetLineRenderer();
			lineRenderer.SetPosition(0, mousePosition);
			previousMousePosition = mousePosition;
		}

		// No longer drawing
		else if (Input.GetMouseButtonUp(0))
		{

			isDrawing = false;
			previousLine = line;
			line = null;
		}
		
		// Undo
		else if (Input.GetMouseButtonUp(1) && !isDrawing)
		{
			Destroy(previousLine);
			foreach (var collider in previousLineColliders) {
				Destroy(collider);
			}
			previousLineColliders.Clear();
		}

		// Draw line
		if (isDrawing)
		{
			// Check for mouse movement
			if (Input.mousePosition.x == previousMouseCoordinates.x &&
				Input.mousePosition.y == previousMouseCoordinates.y) {
				return;
			}

			// Check that surface is drawable
			var hit = GetHit();
			if (hit.collider == null || !hit.collider.CompareTag("Drawable")) {
				return;
			}

			var mousePosition = GetMousePosition(hit);
			var lineRenderer = GetLineRenderer();
			if (lineRenderer == null) {
				return;
			}
			lineRenderer.positionCount++;
			lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosition);
            previousMouseCoordinates = Input.mousePosition;

            // Create collider container
            var colliderContainer = new GameObject("Collider");

			// Create collider
			var boxCollider = colliderContainer.AddComponent<BoxCollider>();
			colliderContainer.transform.position = Vector3.Lerp(
				previousMousePosition, mousePosition, 0.2f);
			colliderContainer.transform.LookAt(mousePosition);
			boxCollider.size = new Vector3(2f, 0.1f,
				Vector3.Distance(previousMousePosition, mousePosition));
            colliderContainer.tag = "Line";

			previousLineColliders.Add(colliderContainer);
			previousMousePosition = mousePosition;
		}
	}


	RaycastHit GetHit() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		return hit;
	}

	Vector3 GetMousePosition(RaycastHit hit) {
        return new Vector3(hit.point.x, hit.point.y, hit.point.z - 0.5f);
	}

	LineRenderer GetLineRenderer() {
		if (line == null) {
			return null;
		}
		return line.GetComponent<LineRenderer>();
	}
}
