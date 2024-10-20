﻿using EscapeRoomJam4.QuantumPuzzle;
using System;
using UnityEngine;

namespace EscapeRoomJam4;

public class ERQuantumPuzzleController : MonoBehaviour
{
    private static SocketedQuantumObject _quantumObject1, _quantumObject2, _quantumObject3;

    public Action Solved;

    public void Start()
    {
        _quantumObject1 = transform.Find("QuantumStatue1").GetComponent<SocketedQuantumObject>();
        _quantumObject2 = transform.Find("QuantumStatue2").GetComponent<SocketedQuantumObject>();
        _quantumObject3 = transform.Find("QuantumStatue3").GetComponent<SocketedQuantumObject>();

        // Recolour for test
        _quantumObject1.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        _quantumObject2.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        _quantumObject3.GetComponentInChildren<MeshRenderer>().material.color = Color.green;

        // Can't have 4 objects in 4 slots
        // Instead we have a duplicate of the final object for each slot, which appears when that slot is "empty"
        for (int i = 0; i < _quantumObject1._socketList.Count; i++)
        {
            var emptySocketObject = transform.Find($"QuantumStatueEmptyState{i + 1}").gameObject;
            var socket = _quantumObject1._socketList[i];
            socket._emptySocketObject = emptySocketObject;
            emptySocketObject.SetActive(socket._quantumObject == null);
            emptySocketObject.transform.parent = socket.transform;
            emptySocketObject.transform.localPosition = Vector3.zero;
            emptySocketObject.transform.localRotation = Quaternion.identity;

            // Need to add a visibility tracker for this socket else it doesn't stay "empty" when photographed
            socket.SetActive(false);
            var tracker = new GameObject("VisibilityTracker");
            tracker.transform.parent = socket.transform;
            tracker.transform.localPosition = Vector3.zero;
            tracker.transform.localRotation = Quaternion.identity;
            var box = tracker.AddComponent<BoxShape>();
            box.size = new Vector3(0.2f, 0.6f, 0.2f);
            box.center = new Vector3(0, 0.3f, 0);
            tracker.AddComponent<ShapeVisibilityTracker>();
            // Using a quantum object bc it can be locked by camera
            socket._visibilityObject = socket.gameObject.AddComponent<SnapshotLockableVisibilityObject>();
            socket.SetActive(true);

            // Recolour for test
            emptySocketObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }

        EscapeRoomJam4.WriteDebug($"{nameof(ERQuantumPuzzleController)} - The correct answer is red, blue, green, yellow");
    }

    /// <summary>
    /// Call this from an interaction point
    /// Make sure to add to the Solved action to make it drop a key
    /// </summary>
    public void CheckIfSolved()
    {
        if (TestStates())
        {
            EscapeRoomJam4.WriteDebug($"{nameof(ERQuantumPuzzleController)} - puzzle complete!");
            _quantumObject1.SetIsQuantum(false);
            _quantumObject2.SetIsQuantum(false);
            _quantumObject3.SetIsQuantum(false);

            Solved?.Invoke();
        }
    }

    private bool TestStates()
    {
        EscapeRoomJam4.WriteDebug($"{nameof(ERQuantumPuzzleController)} - Testing states");

        return
            _quantumObject1._occupiedSocket.name == "Socket 0" &&
            _quantumObject2._occupiedSocket.name == "Socket 1" &&
            _quantumObject3._occupiedSocket.name == "Socket 2";
    }
}
