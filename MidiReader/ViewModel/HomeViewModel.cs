using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Melanchall.DryWetMidi.Core;
using GalaSoft.MvvmLight;
using Melanchall.DryWetMidi.Interaction;
using System.Threading.Tasks;
using System.Text;
using System;
using GalaSoft.MvvmLight.Command;
using Melanchall.DryWetMidi.Devices;
using MidiReader.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;

namespace MidiReader.ViewModel
{
    public class HomeViewModel: ViewModelBase
    {        
        //60000 / (BPM * PPQ)

        public HomeViewModel()
        {
            LoadTracks();
            PlayMusicCommand = new RelayCommand(PlayMusic, true);
            PlayNoteCommand = new RelayCommand(PlayNoteMusic, true);
            AddCriteriaCommand = new RelayCommand(AddCriteria, true);
            RemoveCriteriaCommand = new RelayCommand(RemoveCriteria, true);
            ReplaceNoteCommand = new RelayCommand(ReplaceNote, true);
        }


        #region Commands

        private void RemoveCriteria()
        {
            if (ReplacementNotes != null)
            {
                ReplacementNotes.Remove(ReplacementNoteItem);
                
            }
        }

        private void ReplaceNote()
        {            
            MidiHelper.ReplaceNote(Notes,ReplacementNotes);
            RaisePropertyChanged("Notes");
            GroupNotes();
        }

        private void AddCriteria()
        {
            if (ReplacementNotes==null)
            {
                ReplacementNotes = new ObservableCollection<ReplacementNote>();
            }
            ReplacementNotes.Add(new ReplacementNote(SelectedNoteName,SelectedOctave,SelectedReplaceNoteName,SelectedReplaceOctave));           
        }

        private void PlayMusic()
        {
            MidiHelper.PlayTrack(SelectedTrack);
        }

        private void PlayNoteMusic()
        {
            MidiHelper.PlayNotes(SelectedMusicalNote.MusicalNotes);
            //SetTimer();
        }

        public RelayCommand PlayNoteCommand { get; private set; }
        public RelayCommand PlayMusicCommand { get; private set; }
        public RelayCommand AddCriteriaCommand { get; private set; }
        public RelayCommand RemoveCriteriaCommand { get; private set; }
        public RelayCommand ReplaceNoteCommand { get; private set; }

        #endregion
        private async Task getNotesAsync()
        {
            
            StatusMessage = "Gettings Notes...";
            Notes = await MidiHelper.NotesListAsync(SelectedTrack);
            GroupNotes();
            StatusMessage = "Process complete,Ready";
        }

        private void GroupNotes()
        {
            ObservableCollection<MusicalNoteItem> notesColl = new ObservableCollection<MusicalNoteItem>();
            List<Note> noteCollection = null;
            int k = 1;
            foreach (var note in Notes)
            {
                if (k % 16 == 0)
                {
                    noteCollection.Add(note);
                    notesColl.Add(new MusicalNoteItem(noteCollection));
                    noteCollection = new List<Note>();

                }
                else
                {
                    if (noteCollection == null)
                    {
                        noteCollection = new List<Note>();
                    }
                    noteCollection.Add(note);
                }


                k++;
            }
            Musicalnotes = notesColl;
            SelectedMusicalNote = notesColl.FirstOrDefault();
        }
        //private void SetTimer()
        //{
        //    index = 1;
        //    SelectedMusicalNote.SelectedNote = SelectedMusicalNote.MusicalNotes.FirstOrDefault();
        //   if( SelectedMusicalNote.SelectedNote.Time!=0.0)
        //    {
        //        MetricTimeSpan metricTime = SelectedMusicalNote.SelectedNote.LengthAs<MetricTimeSpan>(tempoMap);
        //        aTimer = new System.Timers.Timer(metricTime.Milliseconds);
        //        aTimer.Interval = SelectedMusicalNote.MusicalNotes.FirstOrDefault().Time;
        //    }
        //    else
        //    {
        //        aTimer = new System.Timers.Timer(10);
        //        aTimer.Interval = 10;
        //    }

        //    aTimer.Elapsed += OnTimedEvent;
        //    aTimer.AutoReset = true;
        //    aTimer.Enabled = true;
        //}

        //private void OnTimedEvent(object sender, ElapsedEventArgs e)
        //{
        //    if (index==16)
        //    {
        //        aTimer.Stop();
        //        aTimer.Dispose();
        //        return;
        //    }
        //    SelectedMusicalNote.SelectedNote = SelectedMusicalNote.MusicalNotes.ElementAt(index);
        //    MetricTimeSpan metricTime = SelectedMusicalNote.SelectedNote.TimeAs<MetricTimeSpan>(tempoMap);
        //    MetricTimeSpan metricLen = SelectedMusicalNote.SelectedNote.LengthAs<MetricTimeSpan>(tempoMap);
        //    if (metricTime.Milliseconds==0)
        //    {
        //        aTimer.Interval = metricLen.Milliseconds;
        //    }
        //    else 
        //        aTimer.Interval=metricTime.Milliseconds;
        //    index++;
        //}

       

        
        private void LoadTracks()
        {
            MidiFile mFile = MidiHelper.ReadMidi(@"E:\Downloads\teddybear.mid");
            tracks = MidiHelper.GetTracks(mFile);

            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Tracks Loaded."));
        }

        #region properties

        private string statusMessage;
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                RaisePropertyChanged("StatusMessage");
            }
        }

        private string selectedNoteName;
        public string SelectedNoteName
        {
            get
            {
                return selectedNoteName;
            }
            set
            {
                selectedNoteName = value;
                RaisePropertyChanged("SelectedNoteName");
            }
        }
        private int selectedOctave;
        public int SelectedOctave
        {
            get
            {
                return selectedOctave;
            }
            set
            {
                selectedOctave = value;
                RaisePropertyChanged("SelectedOctave");
            }
        }

        private string selectedReplaceNoteName;
        public string SelectedReplaceNoteName
        {
            get
            {
                return selectedReplaceNoteName;
            }
            set
            {
                selectedReplaceNoteName = value;
                RaisePropertyChanged("SelectedReplaceNoteName");
            }
        }
        private int selectedReplaceOctave;
        public int SelectedReplaceOctave
        {
            get
            {
                return selectedReplaceOctave;
            }
            set
            {
                selectedReplaceOctave = value;
                RaisePropertyChanged("SelectedReplaceOctave");
            }
        }

        private IEnumerable<TrackChunk> tracks;
        public IEnumerable<TrackChunk> Tracks
        {
            get
            {
                return tracks;
            }
            set
            {
                tracks = value;
                RaisePropertyChanged("Tracks");
            }
        }
   
        private NotesCollection notes;
        public NotesCollection Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        
        private MusicalNoteItem selectedMusicalNote;
        public MusicalNoteItem SelectedMusicalNote
        {
            get
            {
                return selectedMusicalNote;
            }
            set
            {
                if(selectedMusicalNote!=null)
                    selectedMusicalNote.SelectedNote = null;
                selectedMusicalNote = value;
                RaisePropertyChanged("SelectedMusicalNote");
                if (selectedMusicalNote != null)
                    selectedMusicalNote.SelectedNote = null;
            }
        }

        private ObservableCollection<ReplacementNote> replacementNotes;
        public ObservableCollection<ReplacementNote> ReplacementNotes
        {
            get
            {
                return replacementNotes;
            }
            set
            {
                replacementNotes = value;
                RaisePropertyChanged("ReplacementNotes");
            }
        }
        private ObservableCollection<MusicalNoteItem> musicalnotes;
        public ObservableCollection<MusicalNoteItem> Musicalnotes
        {
            get
            {
                return musicalnotes;
            }
            set
            {
                musicalnotes = value;
                RaisePropertyChanged("Musicalnotes");
            }
        }

        private string notesString;
        public string NotesString
        {
            get
            {
                return notesString;
            }
            set
            {
                notesString = value;
                RaisePropertyChanged("NotesString");
            }
        }

        private TrackChunk selectedTrack;
        public TrackChunk SelectedTrack
        {
            get
            {
                return selectedTrack;
            }
            set
            {
                selectedTrack = value;
                RaisePropertyChanged("SelectedTrack");

                getNotesAsync();//getTracks();
                MidiHelper.StopPlay();
            }
        }  //logic to update Notes on basis of selected Track.

        public ReplacementNote ReplacementNoteItem { get; set; }



        #endregion




    }
}
