using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class represents a one directional portal
 * 
 * PROD321 - Interactive Computer Graphics and Animation 
 * Copyright 2024, University of Canterbury
 * Written by Adrian Clark
 */

public class Portal : MonoBehaviour
{
    // The region that this portal links to
    public Region nextRegion;

    // A Dictionary of FrustumGameObjects for each PortalCulling Object
    [HideInInspector]
    public Dictionary<PortalCulling, GameObject> frustumGameObjects = new Dictionary<PortalCulling, GameObject>();

    // The colour of this portals frustum
    public Color portalFrustumColour;
    
    // The vertices which define this portal
    [HideInInspector]
    public List<Vector3> vertices;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure that the local scale is positive
        Vector3 localScale = transform.localScale;
        if (localScale.x < 0) localScale.x *= -1;
        if (localScale.y < 0) localScale.y *= -1;
        if (localScale.z < 0) localScale.z *= -1;
        transform.localScale = localScale;

        // Get the mesh filter of this portal
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

        // Get the vertices which define the outside edges of our portal
        vertices = ContinuousEdgeList.GetContinuousEdgeList(meshFilter.mesh.vertices, meshFilter.mesh.triangles, 0.0001f);

        // Calculate the normal of our portal based on two of the calculated
        // outside edges
        Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[1] - vertices[2]);

        // If the normal is pointing opposite to the normal of the mesh
        // reverse the order of our calculated outside edges - this way we
        // know our calculated outside edges are in correct winding order
        if (Vector3.Dot(meshFilter.mesh.normals[0], normal) < 0)
            vertices.Reverse();
    }

    void CreateFrustumGameObject(PortalCulling portalCulling)
    {
        // Create the frustum for this portal
        GameObject frustumGO = new GameObject("Portal Frustum : " + transform.parent.name + " - " + nextRegion.name);
        // Put it under the frustum container defined by the portal culling script
        frustumGO.transform.SetParent(portalCulling.frustumContainer, false);
        // Set it's render layer to the render laying of the portal culling gameobject
        frustumGO.layer = portalCulling.gameObject.layer;
        // Set it's position and rotation to the identity
        frustumGO.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Add this frustum to the dictionary
        frustumGameObjects.Add(portalCulling, frustumGO);
    }

    // Called if we want to hide the frustum
    public void HideFrustum(PortalCulling portalCulling)
    {
        if (frustumGameObjects.ContainsKey(portalCulling))
        {
            // Get our frustum's mesh renderer
            MeshRenderer mr = frustumGameObjects[portalCulling].GetComponent<MeshRenderer>();
            // If it exists, disable it
            if (mr != null) mr.enabled = false;
        }
    }

    // Get an updated frustum for this portal
    public Frustum GetUpdatedFrustum(PortalCulling portalCulling, Camera c)
    {
        // Get our portal culling script
        if (!frustumGameObjects.ContainsKey(portalCulling))
            CreateFrustumGameObject(portalCulling);

        // Create a frustum from this portal, using the cameras position,
        // our frustum game object, the material defined in the portal culling script
        // and the colour of this portal's frustum
        return Frustum.CreateFrustumFromPortal(this, c, frustumGameObjects[portalCulling], portalCulling.frustumMaterial, portalFrustumColour);
    }
}
