let audio = null;
let currentSrc = null;
let savedSrc = null;
let savedTime = 0;

const PREF_KEY = 'is_audio_enabled';

export function loadPreference() {
    const val = localStorage.getItem(PREF_KEY);
    return val === null || val === 'true';
}

export function savePreference(enabled) {
    localStorage.setItem(PREF_KEY, enabled ? 'true' : 'false');
}

function ensureAudio() {
    if (!audio) {
        audio = new Audio();
        audio.loop = true;
    }
    return audio;
}

export function play(src) {
    const a = ensureAudio();
    if (src === savedSrc) {
        if (currentSrc !== src) {
            a.src = src;
            currentSrc = src;
        }
        a.currentTime = savedTime;
    } else if (currentSrc !== src) {
        a.src = src;
        currentSrc = src;
    }
    savedSrc = null;
    savedTime = 0;
    a.play().catch(() => {});
}

export function saveAndPause() {
    if (audio && !audio.paused) {
        savedSrc = currentSrc;
        savedTime = audio.currentTime;
        audio.pause();
    }
}

export function stop() {
    currentSrc = null;
    savedSrc = null;
    savedTime = 0;
    if (audio) {
        audio.pause();
        audio.src = '';
    }
}
