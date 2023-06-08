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
    private Dictionary<int, Action<float>> _noteOnActions = new Dictionary<int, Action<float>>();

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/midi";

    public float scaleAmount = 5f;

    private Dictionary<int, Coroutine> _scaleCoroutines = new Dictionary<int, Coroutine>();

    void Start()
    {
        RegisterEndpoints();
        _moveables.AddRange(FindObjectsOfType<Moveable>());
        
        for (int i = 0; i < _moveables.Count; i++)
        {
            var moveable = _moveables[i];
            _noteOnActions.Add(i, (float velocity) => {

                
                
                Debug.Log("Note On Action Called " + velocity);
                // var currentSnapshot = moveable.CurrentSnapshot;
                Vector3 originalScale = moveable.DefaultSnapshot.Scale;

                // var originalPosition = currentSnapshot.Position;

                // Debug.Log("Sanity Check " + (float)originalScale.x * (1f + velocity));

                // Vector3 newScale = new Vector3(1 + ((float)originalScale.x * (1f + velocity)), 1 + ((float)originalScale.y * (1f + velocity)), 1 + ((float)originalScale.z * (1f + velocity)));
                // newScale *= scaleAmount;

                // Debug.Log("New Scale " + newScale);

                // // moveable.ScaleTo(newScale, 0.3f);
                // moveable.gameObject.transform.localScale = newScale;

                if (_scaleCoroutines.ContainsKey(i))
                    StopCoroutine(_scaleCoroutines[i]);
                _scaleCoroutines[i] = StartCoroutine(ScaleTo(moveable, originalScale, 0.1f, 0.3f));
                // moveable.MoveTo(newPos, 0.7f);
                // CoroutineHelpers.DelayedAction(() => {
                //     Debug.Log("Resetting Scale to " + originalScale);
                //     // moveable.ScaleTo(originalScale, 0.5f);
                //     moveable.gameObject.transform.localScale = originalScale;
                // }, 0.3f, this);
            });
        }
    }

    private IEnumerator ScaleTo(Moveable moveable, Vector3 newScale, float durationAttack, float durationDecay)
    {
        // var originalScale = moveable.CurrentSnapshot.Scale;
        Vector3 originalScale = moveable.DefaultSnapshot.Scale;

        newScale = moveable.gameObject.transform.localScale * scaleAmount;
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / durationAttack;
            moveable.gameObject.transform.localScale = Vector3.Lerp(moveable.gameObject.transform.localScale, newScale, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / durationDecay;
            moveable.gameObject.transform.localScale = Vector3.Lerp(newScale, originalScale, t);
            yield return null;
        }
    }

    // idea : add a camera controller
    // idea : add support for ps4 controller
    public void RegisterEndpoints()
    {

        OscManager.Instance.AddEndpoint($"{OscAddress}/note", (OscDataHandle dataHandle) => {
            // Debug.Log("Note Received");
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
        _noteOnActions[note]?.Invoke(velocity);
    }

    void NoteOff(int note, float velocity)
    {
        // if not is in activeNotes    
        if (_activeNotes.ContainsKey(note)) {
            _activeNotes.Remove(note);
            _noteOnActions[note]?.Invoke(velocity);
        }
    }

}
