using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;
using System;

public class PolyphonicMidiController : MonoBehaviour, IOscControllable, IArtworkController
{
    List<Moveable> _moveables = new List<Moveable>();
    Dictionary<int, float> _activeNotes = new Dictionary<int, float>();
    private Dictionary<int, Action<int, float>> _noteOnActions = new Dictionary<int, Action<int, float>>();

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Index}/midi";

    public float scaleAmount = 5f;
    public float sizeAttackTime = 0.1f;
    public float sizeDecayTime = 1.5f;

    private Dictionary<int, Coroutine> _scaleCoroutines = new Dictionary<int, Coroutine>();

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void Start()
    {
        _moveables.AddRange(FindObjectsOfType<Moveable>());
        
        for (int i = 0; i < _moveables.Count; i++)
        {

            // IDEA - we need to get these at their normalized index
            // basically all of these controllers should be accessing the Motif object and not the moveable
            // the motif can provide access to all of the apis we need to move and scale things

            // var moveable = _moveables[i];

            _noteOnActions.Add(i, (int noteIndex, float velocity) => {

                Vector3 originalScale = _moveables[noteIndex].DefaultSnapshot.Scale;

                if (_scaleCoroutines.ContainsKey(noteIndex))
                    StopCoroutine(_scaleCoroutines[noteIndex]);

                _scaleCoroutines[noteIndex] = StartCoroutine(
                    ScaleTo(
                        _moveables[noteIndex].gameObject, 
                        originalScale * (1f + velocity) * scaleAmount, 
                        originalScale, 
                        sizeAttackTime,
                        sizeDecayTime)
                    );
            });
        }
    }

    private IEnumerator ScaleTo(GameObject go, Vector3 newScale, Vector3 originalScale, float durationAttack, float durationDecay)
    {
        var t = 0f;
        Vector3 initialScale = go.transform.localScale;



        
        while (t < durationAttack)
        {
            t += Time.deltaTime / durationAttack;
            go.transform.localScale = Vector3.Lerp(initialScale, newScale, t);
            yield return null;
        }
        go.transform.localScale = newScale;
        t = 0f;
        while (t < durationDecay)
        {
            t += Time.deltaTime / durationDecay;
            go.transform.localScale = Vector3.Lerp(newScale, originalScale, t);
            yield return null;
        }
        go.transform.localScale = originalScale;
    }

    public void RegisterEndpoints()
    {

        OscManager.Instance.AddEndpoint($"{OscAddress}/note", (OscDataHandle dataHandle) => {
            var note = dataHandle.GetElementAsInt(0);
            var velocity = (float)dataHandle.GetElementAsInt(1)/127f;
            var channel = dataHandle.GetElementAsInt(2);
            
            Debug.Log($"Note: {note} Velocity: {velocity} Channel: {channel}");

            NoteIn(note-43, 1f, channel);
        });
    }

    void NoteIn(int note, float velocity, int channel)
    {
        if (velocity > 0) {
            NoteOn(note, velocity);
        }

        if (velocity == 0) {
            NoteOff(note, velocity);
        }
    }


    void NoteOn(int note, float velocity)
    {
        _activeNotes[note] = velocity;
        if(!_noteOnActions.ContainsKey(note)) {
            _noteOnActions[note] = null;
        }
        _noteOnActions[note]?.Invoke(note, velocity);
    }

    void NoteOff(int note, float velocity)
    {
        // if not is in activeNotes    
        if (_activeNotes.ContainsKey(note)) {
            _activeNotes.Remove(note);
            _noteOnActions[note]?.Invoke(note, velocity);
        }
    }

}
