﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnScript : MonoBehaviour {

    public List<UnitScript> units;
    public List<UnitScript> PCunits;
    public UnitScript activeUnit;
    public float gametime;

    public static TurnScript instance;

    // increment turn idle time and send first unit to its new turn order spot
    public void NextTurn()
    {
        units.RemoveAt(0);

        foreach (UnitScript u in units) u.idle++;

        AddUnitOrdered(activeUnit);

        activeUnit = units[0];
    }

    // Add unit to turn order based on its idle time and initiative
    private void AddUnitOrdered(UnitScript unit)
    {
        unit.idle = 1;
        int turnWeight = unit.TurnWeight();
        for (int i = units.Count-1; i >= 0; i--)
        {
            if (units[i].TurnWeight() >= turnWeight)
            {
                units.Insert(i + 1, unit);
                return;
            }
        }
        units.Insert(0, unit);
    }

    //TODO: init actual tilemap
    public MovementScript.TileMapPlaceholder tilemap;

    // Initialize turn order and find the current active unit
    void Start () {
        instance = this;

        // Turn order and movement testing
        tilemap = new MovementScript.TileMapPlaceholder(20,20);

        //built tile graph
        MovementScript.BuildPathGraph(tilemap);


        List<UnitScript> temporary = new List<UnitScript>(units);
        units.Clear();
        foreach (UnitScript unit in temporary) {
            AddUnitOrdered(unit);
            unit.tile = tilemap.tiles[(int)unit.transform.position.x, (int)unit.transform.position.y];

            //add all non AI controlled units to a player list, this will be the target for enemy units
            if (unit.GetComponent<EnemyScript>() == null) PCunits.Add(unit);
        }
        activeUnit = units[0];






    }

    // Turn order and movement testing
    void Update () {
        if (gametime < Time.fixedTime - 2f)
        {
            /*List<MovementScript.TilePlaceholder> possible_tiles = activeUnit.move_calc.CalculateMovement(tilemap, activeUnit.tile, activeUnit.mP);
            List<MovementScript.TilePlaceholder> path = activeUnit.move_calc.FindPathTo(tilemap, possible_tiles[Random.Range(0, possible_tiles.Count)]);
            activeUnit.move_calc.MoveToTile(path, activeUnit);*/

            if (activeUnit.GetComponent<EnemyScript>() != null) activeUnit.GetComponent<EnemyScript>().AITurn();
            else activeUnit.GetComponent<AllyScript>().PlayerTurn();

            Debug.Log("--------");

            NextTurn();
            gametime = Time.fixedTime;
        }
	}
}
