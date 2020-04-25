using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiReader.Models;
using System.Collections.ObjectModel;

namespace MidiReader
{
    public class MidiHelper
    {
        private static TempoMap tempoMap;
        static Playback trackPlayback;
        static OutputDevice playDevice;
        public static MidiFile ReadMidi(string filePath)
        {
            
            MidiFile midiFile = MidiFile.Read(filePath); //@"E:\Downloads\teddybear.mid"
            tempoMap = midiFile.GetTempoMap();
            
            return midiFile;
        }

        public static void StopPlay()
        {
            trackPlayback?.Stop();
        }
        public static IEnumerable<TrackChunk> GetTracks(MidiFile midiFile)
        {
            return midiFile.Chunks.OfType<TrackChunk>();
        }
        public static NotesCollection NotesList(TrackChunk trackChunk)
        {
            NotesCollection notes;
            using (var notesManager = new NotesManager(trackChunk.Events))
            {
                notes = notesManager.Notes;

            }
            return notes;
        }

        public static void ReplaceNote(NotesCollection notes, ObservableCollection<ReplacementNote> replacementNotes)
        {
            foreach (var item in replacementNotes)
            {
                var filterList = notes.Where(x => x.NoteNumber == item.NoteNumber).ToList();
                foreach (var note in filterList)
                {
                    note.NoteNumber = item.ReplacementNoteNumber;
                    //note.Octave = item.ReplacementOctave;
                    //note.NoteName = item.ReplacementNoteName;                    
                }
            }
           
                //.Select(c => { c.NoteNumber = newNoteN; return c; });
            
        }

        public static Task<NotesCollection> NotesListAsync(TrackChunk trackChunk)
        {
            NotesCollection notes;
            
            return Task<NotesCollection>.Factory.StartNew(() =>
            {
                using (var notesManager = new NotesManager(trackChunk.Events))
                {
                    notes = notesManager.Notes;

                }
                return notes;
            });

        }
        public static IEnumerable<ITimedObject> GetNotesList(TrackChunk trackChunk)
        {
            return trackChunk.GetNotesAndRests(RestSeparationPolicy.SeparateByNoteNumber);
            
        }
        public static EventsCollection GetAllNotes(TrackChunk trackChunk)
        {
            return trackChunk.Events;
        }
        public static void PlayMidi(MidiFile midiFile)
        {
            if (playDevice == null)
            {
                setPlayDevice();
            }
            trackPlayback?.Stop();
            //playDevice?.Dispose();

            midiFile.Play(playDevice);
            
        }

        public static void PlayNotes(IEnumerable<Note> notes)
        {
            trackPlayback?.Stop();
            //playDevice?.Dispose();

            IEnumerable<Note> editedNotes = new List<Note>();
            editedNotes= UpdateTimeForNotes(notes);
            TrackChunk myTrack = editedNotes.ToTrackChunk();

            //NotesManagingUtilities.AddNotes(myTrack, SelectedMusicalNote.MusicalNotes);    
            if (playDevice==null)
            {
                setPlayDevice();
            }        
            trackPlayback = PlaybackUtilities.GetPlayback(myTrack, tempoMap, playDevice, null);
            if (!trackPlayback.IsRunning)
                trackPlayback.Start();
        }

        private static IEnumerable<Note> UpdateTimeForNotes(IEnumerable<Note> musicalNotes)
        {
            IEnumerable<Note> newNote = musicalNotes;
            long time = newNote.FirstOrDefault().Time;
            foreach (var note in newNote)
            {
                note.Time -= time;
            }
            return newNote;
        }

        public static void PlayTrack(TrackChunk track)
        {
            trackPlayback?.Stop();
            //playDevice?.Dispose();

            if (playDevice == null)
            {
                setPlayDevice();
            }
            trackPlayback = PlaybackUtilities.GetPlayback(track, tempoMap, playDevice, null);
            trackPlayback.Start();
        }

        public static void PlaySingleNote(Note note)
        {
            if (note == null)
                return;
            Note newNote = new Note((SevenBitNumber)10);
            newNote=note.Clone();
            trackPlayback?.Stop();
            //playDevice?.Dispose();

            newNote.Time = 0;
            List<Note> singleNoteList = new List<Note>();
            singleNoteList.Add(newNote);

            TrackChunk myTrack = singleNoteList.ToTrackChunk();

            //NotesManagingUtilities.AddNotes(myTrack, SelectedMusicalNote.MusicalNotes);
            if (playDevice == null)
            {
                setPlayDevice();
            }
            trackPlayback = PlaybackUtilities.GetPlayback(myTrack, tempoMap, playDevice, null);
            if (!trackPlayback.IsRunning)
                trackPlayback.Start();
        }

        private static void setPlayDevice()
        {
            string devName = string.Empty;
            if (playDevice==null)
            {
                foreach (var outputDevice in OutputDevice.GetAll())
                {
                    devName = outputDevice.Name;
                    break;
                }
                playDevice = OutputDevice.GetByName(devName);
            }
           
        }

        public static IEnumerable<Note> ReplaceNote(IEnumerable<Note> Notes,SevenBitNumber NoteN,SevenBitNumber newNoteN)
        {
            Notes.Where(x => x.NoteNumber == NoteN).ToList().Select(c => { c.NoteNumber = newNoteN; return c; });
            return Notes;            
        }
    }
}
