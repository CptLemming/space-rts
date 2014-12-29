using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour {

    protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    private bool currentlySelected = false;
    private Bounds selectionBounds;

	// Use this for initialization
	void Start () {
        CalculateBounds();
        playingArea = GetPlayingArea();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void SetSelection(bool selected) {
        currentlySelected = selected;
    }

    protected virtual void OnGUI() {
        if (currentlySelected) {
            DrawSelection();
        }
    }

    public Rect GetPlayingArea() {
        return new Rect(0, 0, Screen.width, Screen.height);
    }

    public void CalculateBounds() {
        selectionBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
            selectionBounds.Encapsulate(r.bounds);
        }
    }

    private void DrawSelection() {
        //GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = CalculateSelectionBox(selectionBounds, playingArea);
        // Draw the selection box around the currently selection object, within the bounds of the playing area
        GUI.BeginGroup(playingArea);
        DrawSelectionBox(selectBox);
        GUI.EndGroup();
    }

    protected virtual void DrawSelectionBox(Rect selectBox) {
        GUI.Box(selectBox, "");
    }

    private Rect CalculateSelectionBox(Bounds selectionBounds, Rect playingArea) {
        // Shorthand for the coordinates of the centre of the selection bounds
        float cx = selectionBounds.center.x;
        float cy = selectionBounds.center.y;
        float cz = selectionBounds.center.z;

        // Shorthand for the coordinates of the extends of the selection bounds
        float ex = selectionBounds.extents.x;
        float ey = selectionBounds.extents.y;
        float ez = selectionBounds.extents.z;

        // Determine the screen coordinates for the corners of the selection bounds
        List<Vector3> corners = new List<Vector3>();
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez)));

        // Determine the bounds on screen for the selection bounds
        Bounds screenBounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < corners.Count; i++) {
            screenBounds.Encapsulate(corners[i]);
        }

        // Screen coordinates start in the bottom left corner, rather than the top left corner
        // this correction is needed to make sure the selection box is drawn in the correct place
        float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
        float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
        float selectBoxWidth = 2 * screenBounds.extents.x;
        float selectBoxHeight = 2 * screenBounds.extents.y;

        return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
    }
}
