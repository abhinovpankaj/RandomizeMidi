using GalaSoft.MvvmLight;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiReader.Models
{
    public class MusicalNoteItem:ViewModelBase
    {
        public IEnumerable<Note> MusicalNotes { get; set; }
        public string NoteString { get; set; }
        public Note FirstNote { get; set; }

        private Note selectedNote;
        public Note SelectedNote
        {
            get
            {
                return selectedNote;
            }
            set
            {
                selectedNote = value;
                RaisePropertyChanged("SelectedNote");
                MidiHelper.PlaySingleNote(selectedNote);
            }
        }
        public MusicalNoteItem(IEnumerable<Note> notes)
        {
            MusicalNotes = notes;
            CreateNotestring();
        }

        private void CreateNotestring()
        {           
            StringBuilder strBuilder = new StringBuilder();
            foreach (var note in MusicalNotes)
            {
                string noteName = note.NoteName.ToString().Replace("Sharp", "#");
                strBuilder.Append(noteName + "  ");               
            }
            NoteString = strBuilder.ToString();
        }
    }
}
