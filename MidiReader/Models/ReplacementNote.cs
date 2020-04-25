using Melanchall.DryWetMidi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiReader.Models
{
   public class ReplacementNote
    {
        
        public ReplacementNote(string selectedNoteName, int selectedOctave, string selectedReplaceNoteName, int selectedReplaceOctave)
        {
            NoteName = selectedNoteName;
            Octave = selectedOctave;
            ReplacementNoteName = selectedReplaceNoteName;
            ReplacementOctave = selectedReplaceOctave;
        }

        public string NoteName { get; set; }
        public string ReplacementNoteName { get; set; }

        public SevenBitNumber NoteNumber { get { return GetNotenumber(NoteName, Octave); } }
        public int Octave { get; set; }
        public int ReplacementOctave { get; set; }
        public SevenBitNumber ReplacementNoteNumber { get { return GetNotenumber(ReplacementNoteName, ReplacementOctave); } }

        private SevenBitNumber GetNotenumber(string noteName,int octave)
        {
            List<string> noteString = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            int index=noteString.IndexOf(noteName);

            return (SevenBitNumber) ((octave+1)*12 + index);
        }
    }
    
}
