import tensorflow as tf
from magenta.models.melody_rnn import MelodyRnnSequenceGenerator
from nltk.tokenize import word_tokenize
from nltk import download, pos_tag, FreqDist, bigrams
from nltk.corpus import stopwords
from nltk.sentiment.vader import SentimentIntensityAnalyzer
from collections import Counter
import random
import sounddevice as sd
from mido import MidiFile, MidiTrack, Message
import tkinter as tk
from tkinter import filedialog, messagebox
import matplotlib.pyplot as plt
from music21 import converter, instrument, note, chord, stream

# Load the pre-trained Melody RNN model (replace path with your downloaded checkpoint directory)
melody_rnn = MelodyRnnSequenceGenerator(
    checkpoint_dir='./melody_rnn/lstm/basic_rnn',
    bundle_file='basic_rnn.mag'
)

def analyze_lyrics(lyrics):
    """
    Analyzes lyrics for various aspects to understand their structure and tone.

    Args:
        lyrics: A string containing the song lyrics.

    Returns:
        A dictionary containing analysis data.
    """
    try:
        # Download necessary resources
        download('punkt')
        download('averaged_perceptron_tagger')
        download('vader_lexicon')
        download('stopwords')

        # Tokenize and perform Part-of-Speech tagging
        tokens = word_tokenize(lyrics.lower())
        pos_tags = pos_tag(tokens)

        # Analyze word frequency and POS tag distribution
        word_count = Counter(tokens)
        pos_tag_count = Counter(tag for _, tag in pos_tags)

        # Sentiment analysis using Vader lexicon
        sentiment_analyzer = SentimentIntensityAnalyzer()
        sentiment_score = sentiment_analyzer.polarity_scores(lyrics)

        # Analyze bigrams (optional)
        bigram_counts = FreqDist(bigrams(tokens))

        # Return analysis data
        return {
            "word_count": word_count,
            "pos_tag_count": pos_tag_count,
            "sentiment_score": sentiment_score,
            "bigram_counts": bigram_counts  # Optional
        }
    except Exception as e:
        print(f"Error analyzing lyrics: {e}")
        return None

def generate_lyric_suggestion(analysis_data, theme=None, custom_words=None):
    """
    Generates a new lyric line based on word frequency, POS tags, and optionally a theme.

    Args:
        analysis_data: A dictionary containing lyric analysis data.
        theme: An optional string representing the desired theme (e.g., "happy", "sad").
        custom_words: An optional list of custom words to include in the generated lyrics.

    Returns:
        A string containing a new lyric line.
    """
    try:
        word_count = analysis_data["word_count"]
        pos_tag_count = analysis_data["pos_tag_count"]

        # Identify frequent content words (excluding stopwords)
        stop_words = set(stopwords.words('english'))
        content_words = [word for word, count in word_count.most_common(10) if word not in stop_words]

        # Consider custom words
        if custom_words:
            content_words.extend(custom_words)

        # Consider POS tags based on theme (if provided)
        filtered_pos_tags = pos_tag_count.keys()
        if theme:
            filtered_pos_tags = {tag for tag in filtered_pos_tags if tag in ['NN', 'JJ', 'VB']}  # Focus on nouns, adjectives, verbs
            if theme == "happy":
                filtered_pos_tags &= {'JJ': []}  # Exclude negative adjectives for happy theme
            elif theme == "sad":
                filtered_pos_tags &= {'JJ': ['sad', 'lonely', 'gloomy'], 'VB': ['cry', 'miss']}  # Include specific words for sad theme

        # Randomly choose a word, prioritize content words if available
        new_word = random.choice(content_words if content_words else list(word_count.keys()))

        # Consider bigrams for context (optional)
        # You can explore using bigram_counts data here to influence word choice based on frequent phrases

        return new_word
    except Exception as e:
        print(f"Error generating lyric suggestion: {e}")
        return ""

def generate_melody(lyrics):
    """
    Attempts to generate a melody based on preprocessed lyrics using a pre-trained Melody RNN model.

    Args:
        lyrics: A string containing the song lyrics.

    Returns:
        A list of integers representing the generated melody (MIDI format).
    """
    try:
        # Character-based lyric encoding (assuming the model expects this format)
        char_to_int = {char: i for i, char in enumerate(sorted(set(lyrics)))}
        encoded_lyrics = [char_to_int[char] for char in lyrics]

        # Generate melody using the pre-trained model
        input_sequence = tf.train.SequenceExample()
        input_sequence.feature_lists.feature_list['melody'].feature.add().int64_list.value.extend(encoded_lyrics)
        melody = melody_rnn.generate(input_sequence)

        return melody
    except Exception as e:
        print(f"Error generating melody: {e}")
        return []

def save_midi_file(melody, file_path):
    """
    Saves the generated melody as a MIDI file.

    Args:
        melody: A list of integers representing the generated melody (MIDI format).
        file_path: The file path to save the MIDI file.
    """
    try:
        midi = MidiFile()
        track = MidiTrack()
        midi.tracks.append(track)

        for note in melody:
            track.append(Message('note_on', note=note, velocity=64, time=480))
            track.append(Message('note_off', note=note, velocity=64, time=480))

        midi.save(file_path)
    except Exception as e:
        print(f"Error saving MIDI file: {e}")

def play_melody(melody):
    """
    Plays the generated melody using the sounddevice library.

    Args:
        melody: A list of integers representing the generated melody (MIDI format).
    """
    try:
        midi = MidiFile()
        track = MidiTrack()
        midi.tracks.append(track)

        for note in melody:
            track.append(Message('note_on', note=note, velocity=64, time=480))
            track.append(Message('note_off', note=note, velocity=64, time=480))

        midi_bytes = midi.save(file='melody.mid')
        sd.play(midi_bytes)
    except Exception as e:
        print(f"Error playing melody: {e}")

def visualize_melody(melody):
    """
    Visualizes the generated melody using the music21 library.

    Args:
        melody: A list of integers representing the generated melody (MIDI format).
    """
    try:
        s = stream.Stream()
        for pitch in melody:
            n = note.Note(pitch)
            s.append(n)
        s.show('text')
    except Exception as e:
        print(f"Error visualizing melody: {e}")

def generate_chord_progression():
    """
    Generates a basic chord progression that matches the melody.

    Returns:
        A list of chord symbols.
    """
    # Simple I-IV-V-I progression in C major
    return ["C", "F", "G", "C"]

def gui_interface():
    """
    Provides a simple graphical user interface for generating lyrics and melodies.
    """
    def analyze_and_generate():
        lyrics = lyrics_entry.get("1.0", tk.END)
        analysis_data = analyze_lyrics(lyrics)
        if analysis_data:
            theme = theme_var.get()
            custom_words = custom_words_entry.get().split()
            new_lyric = generate_lyric_suggestion(analysis_data, theme, custom_words)
            new_lyric_label.config(text=f"Generated Lyric: {new_lyric}")

            melody = generate_melody(new_lyric)
            if melody:
                visualize_melody(melody)
                save_path = filedialog.asksaveasfilename(defaultextension=".mid", filetypes=[("MIDI files", "*.mid")])
                if save_path:
                    save_midi_file(melody, save_path)
                    messagebox.showinfo("Success", f"Melody saved as {save_path}")
                play_melody(melody)

    root = tk.Tk()
    root.title("Lyric and Melody Generator")

    tk.Label(root, text="Enter Lyrics:").pack()
    lyrics_entry = tk.Text(root, height=10, width=50)
    lyrics_entry.pack()

    tk.Label(root, text="Theme (optional):").pack()
    theme_var = tk.StringVar(value="happy")
    tk.Entry(root, textvariable=theme_var).pack()

    tk.Label(root, text="Custom Words (optional):").pack()
    custom_words_entry = tk.Entry(root)
    custom_words_entry.pack()

    generate_button = tk.Button(root, text="Generate", command=analyze_and_generate)
    generate_button.pack()

    new_lyric_label = tk.Label(root, text="Generated Lyric: ")
    new_lyric_label.pack()

    root.mainloop()

# Example usage:
# Uncomment the line below to run the GUI interface
# gui_interface()
