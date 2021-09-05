#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using TriLibCore.General;

namespace TriLibCore.Samples
{
	public class ImportManager : MonoBehaviour
	{
		// This class manages importing.

		// Manager component
		private ControlManager controlManager;

		// GameObject component
		private GameObject importedBuildings;
		private GameObject standardBoundBuilding;
		private GameObject colliders;

		// Bound variable
		private float minBound;

		// Delete levitation
		private bool deleteLevitation;

		void Start()
        {
			// Get GameObject component
			importedBuildings = GameObject.Find("ImportedBuildings");
			standardBoundBuilding = GameObject.Find("Building.4028");
			minBound = standardBoundBuilding.GetComponent<MeshRenderer>().bounds.size.y;

			// Get manager component
			controlManager = GameObject.Find("ControlManager").GetComponent<ControlManager>();

			// Set collider
			colliders = GameObject.Find("DemRigidbody").transform.GetChild(0).gameObject;
			foreach (var collider in colliders.GetComponents<BoxCollider>())
			{
				collider.isTrigger = true;
			}
			deleteLevitation = false;
		}

		void Update()
        {
			if (deleteLevitation)
            {
				GameObject temp = importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject;
				if (temp.tag == "Untagged")
				{
					temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y - 0.005f, temp.transform.position.z);
				}
				else deleteLevitation = false;
			}
		}

		// Delete levitation
		public void DeleteLevitation(GameObject building)
        {
			building.tag = "Untagged";
			building.transform.position = new Vector3(building.transform.position.x, 0.5f, building.transform.position.z);
			deleteLevitation = true;
		}

		// Set the size of model appropriately
		public void SetSizeOfModel(GameObject building)
        {
			if (!CheckSizeOfModel(building))
            {
				// Calculate ratio and set scale of object
				Bounds bounds = building.GetComponent<MeshRenderer>().bounds;
				float ratio = minBound / bounds.size.x;
				if (minBound / bounds.size.y < ratio) ratio = minBound / bounds.size.y;
				if (minBound / bounds.size.z < ratio) ratio = minBound / bounds.size.z;
				building.transform.localScale = new Vector3(building.transform.localScale.x * ratio, building.transform.localScale.y * ratio, building.transform.localScale.z * ratio);
			}
		}

		// Check the size of model
		public bool CheckSizeOfModel(GameObject building)
        {
			Bounds bounds = building.GetComponent<MeshFilter>().mesh.bounds;
			if (bounds.size.x <= minBound && bounds.size.y <= minBound && bounds.size.z <= minBound) return true;
			else return false;
		}

		// Import model using path
		public void ImportFromPath(string path)
        {
			var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
			assetLoaderOptions.ImportMeshes = true;
			assetLoaderOptions.GenerateColliders = true;
			AssetLoader.LoadModelFromFile(path, OnLoad, null, null, null, importedBuildings, assetLoaderOptions);
		}

		// Import model using file browser
		public void ImportFromBrowser()
		{
			controlManager.SetMode(-1);
			// Set filters (optional)
			// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
			// if all the dialogs will be using the same filters
			FileBrowser.SetFilters(true, new FileBrowser.Filter("3D Models", ".fbx", ".obj", ".stl"));

			// Set default filter that is selected when the dialog is shown (optional)
			// Returns true if the default filter is set successfully
			// In this case, set 3D Models filter as the default filter
			FileBrowser.SetDefaultFilter(".fbx");

			// Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
			// Note that when you use this function, .lnk and .tmp extensions will no longer be
			// excluded unless you explicitly add them as parameters to the function
			FileBrowser.SetExcludedExtensions(".gltf2", ".ply", ".3mf", ".zip");

			// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
			// It is sufficient to add a quick link just once
			// Name: Users
			// Path: C:\Users
			// Icon: default (folder icon)
			FileBrowser.AddQuickLink("Users", "C:\\Users", null);

			// Coroutine example
			StartCoroutine(ShowLoadDialogCoroutine());
		}

		private void OnLoad(AssetLoaderContext assetLoaderContext)
		{
			SetSizeOfModel(importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject);
			importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).position = new Vector3(Camera.main.transform.position.x, 0.5f, Camera.main.transform.position.z);
			Camera.main.transform.eulerAngles = new Vector3(90f, Camera.main.transform.eulerAngles.y, 0f);
			controlManager.SetNormalScale(importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).localScale.x);
		}

		IEnumerator ShowLoadDialogCoroutine()
		{
			yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Files", "Load");

			// Dialog is closed

			if (FileBrowser.Success)
			{
				// Import files
				for (int i = 0; i < FileBrowser.Result.Length; i++)
				{
					ImportFromPath(FileBrowser.Result[i]);
				}
				controlManager.SetMode(1);
			}
            else
            {
				controlManager.SetMode(0);
			}
		}
	}
}