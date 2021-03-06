﻿using GridPlacing;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GridPlacer))]
public class GridPlacer_Editor : Editor
{
    bool showGridMenu = true;
    bool showCreditsMenu = false;
    bool showVisibilityOptions = false;

    GridPlacer gridPlacerScript;

    Grid grid;

    public override void OnInspectorGUI()
    {
        gridPlacerScript = (GridPlacer)target;

        //-- Hide Grid Component
        if (Selection.activeGameObject.GetComponent<Grid>() != null)
            grid = Selection.activeGameObject.GetComponent<Grid>();
        grid.hideFlags = HideFlags.HideInInspector;
        //--

        EditorGUILayout.Space();


        GridPlacingMasterEditor.TitleCreator("GridPlacer", 10, true);
        GridPlacingMasterEditor.DrawUILine(Color.white, 2, 20);

        gridPlacerScript.gridID = EditorGUILayout.IntField("Grid ID", gridPlacerScript.gridID);

        foreach (GridPlacer item in GameObject.FindObjectsOfType<GridPlacer>())
        {
            if (item.gridID == gridPlacerScript.gridID)
            {
                if (item != gridPlacerScript)
                {
                    EditorGUILayout.HelpBox("You can't have same ID in different grids", MessageType.Error);

                    if (EditorApplication.isPlaying)
                    {
                        UnityEditor.EditorApplication.isPlaying = false;
                        Debug.LogError("You can't have same ID in different grids: " + item.name + " || " + gridPlacerScript.name);
                    }
                }
            }
        }

        gridPlacerScript.grid2D = EditorGUILayout.Toggle("Grid2D", gridPlacerScript.grid2D);
        EditorGUILayout.Space();
        showGridMenu = EditorGUILayout.Foldout(showGridMenu, "Grid Options", true);

        if (showGridMenu)
        {
            EditorGUILayout.Space();

            if (gridPlacerScript.grid2D)
            {
                Grid2D();
            }
            else
            {
                Grid3D();
            }

            EditorGUILayout.Space();

            gridPlacerScript.gridCellLayout = (Grid.CellLayout)EditorGUILayout.EnumPopup("Grid Layout", gridPlacerScript.gridCellLayout);
            grid.cellLayout = gridPlacerScript.gridCellLayout;

            /* gridPlacerScript.gridCellSwizzle = (Grid.CellSwizzle)EditorGUILayout.EnumPopup("Grid Swizzle", gridPlacerScript.gridCellSwizzle);
              grid.cellSwizzle = gridPlacerScript.gridCellSwizzle;*/

            gridPlacerScript.keepObjectInGrid = EditorGUILayout.Toggle(new GUIContent("Keep Object in Grid", "Keep object in grid if grid is moved."), gridPlacerScript.keepObjectInGrid);

            gridPlacerScript.placeObjectRelativeToGridPosition = EditorGUILayout.Toggle(new GUIContent("Place Object Relative To Grid Position", "Place Object Relative To Grid Position."), gridPlacerScript.placeObjectRelativeToGridPosition);
        }

        GridPlacingMasterEditor.DrawUILine(Color.gray, 1.5f, 6);

        showVisibilityOptions = EditorGUILayout.Foldout(showVisibilityOptions, "Visibility Options", true);

        if (showVisibilityOptions)
        {
            gridPlacerScript.drawGridIDText = EditorGUILayout.Toggle("Draw Grid ID Text", gridPlacerScript.drawGridIDText);
            gridPlacerScript.drawGridPoints = EditorGUILayout.Toggle("Draw Grid Points", gridPlacerScript.drawGridPoints);

            if (gridPlacerScript.drawGridPoints)
            {
                gridPlacerScript.visibilityRange = EditorGUILayout.Vector2IntField(new GUIContent("Visibility Range", "Don't increase too much or it will lag"), gridPlacerScript.visibilityRange);
                gridPlacerScript.gridPointsColor = EditorGUILayout.ColorField("Points Color", gridPlacerScript.gridPointsColor);
                //gridPlacerScript.radiusGridPoints = EditorGUILayout.FloatField("Points Radius", gridPlacerScript.radiusGridPoints);
                gridPlacerScript.radiusGridPoints = EditorGUILayout.Slider("Radius Grid Points", gridPlacerScript.radiusGridPoints, 0.00f, 0.55f);
            }

            if (GUILayout.Button("Generate Visible Grid"))
            {
                CreateVisibileGrid();
            }
        }

        GridPlacingMasterEditor.DrawUILine(Color.gray, 1.5f, 6);

        showCreditsMenu = EditorGUILayout.Foldout(showCreditsMenu, "Credits", true);

        if (showCreditsMenu)
        {
            GridPlacingMasterEditor.TitleCreator("Made by: Joan Ortiga Balcells", 2, true, TextAnchor.MiddleCenter);
            GridPlacingMasterEditor.TitleCreator("Personal Links:");
            EditorGUILayout.Space();

            if (GUILayout.Button("LinkedIN"))
            {
                Application.OpenURL("www.linkedin.com/in/joanortigabalcells");
            }

            if (GUILayout.Button("Itch.io"))
            {
                Application.OpenURL("https://joanstark.itch.io/");
            }

            if (GUILayout.Button("GitHub"))
            {
                Application.OpenURL("https://github.com/JoanStark");
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUIStyle versionBox = new GUIStyle(EditorStyles.helpBox);


            GUIStyle a = new GUIStyle(EditorStyles.helpBox);

            a.alignment = TextAnchor.MiddleCenter;
            a.fontStyle = FontStyle.Bold;
            a.fontSize = 15;

            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10 + 2));
            r.height = 42;
            r.y += 10 / 2;
            r.x -= 2;
            r.xMin = 0;
            r.width += 6;
            EditorGUI.DrawRect(r, Color.black);

            EditorGUILayout.LabelField("Vesion: 1.0.0", a);

            //DrawUILine(Color.gray, 0.5f, 0);
            //TitleCreator("Vesion: 1.0.0", 1, true, TextAnchor.MiddleCenter);
            //DrawUILine(Color.gray, 0.5f, 0);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Look for Updates"))
            {
                Application.OpenURL("https://github.com/JoanStark");
            }
        }
    }

    private void Grid2D()
    {
        GridPlacingMasterEditor.TitleCreator("2D GRID", 5, true);

        gridPlacerScript.gridCellSize2D = EditorGUILayout.Vector2Field("Grid Cell Size", gridPlacerScript.gridCellSize2D);
        grid.cellSize = gridPlacerScript.gridCellSize2D;

        //Hexagon grid gap is not supported.
        if (gridPlacerScript.gridCellLayout != GridLayout.CellLayout.Hexagon)
        {
            gridPlacerScript.gridCellGap2D = EditorGUILayout.Vector2Field("Grid Cell Gap", gridPlacerScript.gridCellGap2D);
            grid.cellGap = gridPlacerScript.gridCellGap2D;
        }

    }

    private void Grid3D()
    {
        GridPlacingMasterEditor.TitleCreator("3D GRID", 5, true);

        gridPlacerScript.gridCellSize3D = EditorGUILayout.Vector3Field("Grid Cell Size", gridPlacerScript.gridCellSize3D);
        grid.cellSize = gridPlacerScript.gridCellSize3D;

        //Hexagon grid gap is not supported. 
        if (gridPlacerScript.gridCellLayout != GridLayout.CellLayout.Hexagon)
        {
            gridPlacerScript.gridCellGap3D = EditorGUILayout.Vector3Field("Grid Cell Gap", gridPlacerScript.gridCellGap3D);
            grid.cellGap = gridPlacerScript.gridCellGap3D;
        }
    }

    private void CreateVisibileGrid()
    {
        int width = 300;
        int height = 300;

        int widthPixels = 10;

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;

        Color[] colorArray = new Color[width*height];

        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = Color.black;
        }

        texture.SetPixels(0, 0, widthPixels, height, colorArray);
        texture.SetPixels(0, 0, width, widthPixels, colorArray);
        texture.SetPixels(width - widthPixels, 0, widthPixels, height, colorArray);
        texture.SetPixels(0, height - widthPixels, width, widthPixels, colorArray);

        texture.Apply();
    }
}

