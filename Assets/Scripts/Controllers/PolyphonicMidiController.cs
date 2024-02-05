using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;
using System;

public class PolyphonicMidiController : MonoBehaviour, INetworkEndpoint, IArtworkController
{
    Dictionary<int, float> _activeNotes = new Dictionary<int, float>();
    private Dictionary<int, Action<int, float>> _noteOnActions = new Dictionary<int, Action<int, float>>();

    public SegmentedPaintingArtwork Artwork => GetComponent<SegmentedPaintingArtwork>();
    public string Address => $"/artwork/{Artwork.Id}/midi";

    [SerializeField] private float _scaleAmount = 5f;
    [SerializeField] private float _sizeAttackTime = 0.1f;
    [SerializeField] private float _sizeReleaseTime = 1.5f;

    [SerializeField] int _noteRangeHi = 127;
    [SerializeField] int _noteRangeLo = 43;


    [SerializeField] private bool _playAppear = true;
    [SerializeField] private float _playAppearDuration = 1.0f;
    [SerializeField] private int _numNearbyMotifs = 3;
    [SerializeField] private float _nearbyMotifDistanceThresh = 1.0f;

    private Dictionary<int, Coroutine> _scaleCoroutines = new Dictionary<int, Coroutine>();

    // void OnEnable()
    // {
    //     Register();
    // }

    void OnDisable()
    {
        Unregister();
    }

    void OnDestroy()
    {
        Unregister();
    }


    public void Register(string baseAddress)
    {

        OscManager.Instance.AddEndpoint($"{baseAddress}/midi/note", (OscDataHandle dataHandle) => {
            var note = dataHandle.GetElementAsInt(0);
            var velocity = (float)dataHandle.GetElementAsFloat(1);
            var channel = dataHandle.GetElementAsInt(2);
            
            Debug.Log($"Note: {note} Velocity: {velocity} Channel: {channel}");

            NoteIn(note, velocity, channel);
        }, this);
    }

    public void Unregister()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
        // OscManager.Instance.RemoveEndpoint($"{Address}/note");
    }

    void NoteIn(int note, float velocity, int channel)
    {
        if (velocity > 0) {
            NoteOn(note, velocity);
        } else if (velocity == 0) {
            NoteOff(note, velocity);
        }
    }

    private float NormalizedNote(int noteValue) 
    {
        return noteValue / (float)(_noteRangeHi - _noteRangeLo);
    }
    

    void NoteOn(int note, float velocity)
    {
        // Moveable moveable = Artwork.GetMoveableAtNormalizedIndex(NormalizedNote(note));
        Motif motif = Artwork.GetMotifAtNormalizedIndex(NormalizedNote(note));

        motif.ForeachMoveable((moveable) => {
            moveable.EnvelopeScale(1f + velocity * _scaleAmount, _sizeAttackTime, _sizeReleaseTime);
        });

        if (_playAppear) {
            motif.SetOpacitySmooth(1f, 0.0f, _playAppearDuration);
        }


        if (_numNearbyMotifs > 0) 
        {
            var nearbyMotifs = Artwork.GetNearbyMotifs(motif, _numNearbyMotifs, _nearbyMotifDistanceThresh);
            foreach(var otherMotif in nearbyMotifs)
            {
                // make nearby motifs envelope scale, but at a different rate to highlight the main one
                otherMotif.ForeachMoveable((moveable) => {
                    moveable.EnvelopeScale(1f + velocity * _scaleAmount * UnityEngine.Random.Range(0.1f, 0.9f), _sizeAttackTime * 1.15f, _sizeReleaseTime * 0.75f);
                });

                if (_playAppear) {
                    otherMotif.SetOpacitySmooth(1f, 0.0f, _playAppearDuration);
                }
            }
        }
    }

    void NoteOff(int note, float velocity)
    {
    }

}
