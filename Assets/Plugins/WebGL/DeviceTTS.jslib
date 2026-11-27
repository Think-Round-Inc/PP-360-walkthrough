mergeInto(LibraryManager.library, {
  $DeviceTTS: {
    volume: 1,
    rate: 1,
    chunks: [],
    restart: null,
    utterance: null,
    listening: false,

    wake: function () {
      DeviceTTS.trySpeak();
    },

    listen: function () {
      if (DeviceTTS.listening) return;
      DeviceTTS.listening = true;
      window.addEventListener('click', DeviceTTS.wake, true);
      window.addEventListener('keydown', DeviceTTS.wake, true);
      window.speechSynthesis.addEventListener('voiceschanged', DeviceTTS.wake);
    },

    trySpeak: function () {
      if (!DeviceTTS.chunks.length || DeviceTTS.utterance || DeviceTTS.restart !== null) return;
      var synth = window.speechSynthesis;
      var voices = synth.getVoices();
      if (!voices.length) return;

      var voice = voices.find(function (item) {
        return item.localService && /^en(-|$)/i.test(item.lang) && item.default;
      }) || voices.find(function (item) {
        return item.localService && /^en(-|$)/i.test(item.lang);
      }) || voices.find(function (item) {
        return item.localService;
      });
      if (!voice) {
        DeviceTTS.chunks = [];
        return;
      }

      var utterance = new SpeechSynthesisUtterance(DeviceTTS.chunks[0]);
      utterance.voice = voice;
      utterance.volume = DeviceTTS.volume;
      utterance.rate = DeviceTTS.rate;
      DeviceTTS.utterance = utterance;
      function finish(event) {
        if (DeviceTTS.utterance !== utterance) return;
        DeviceTTS.utterance = null;
        if (event.error === 'not-allowed') return;
        if (event.error && event.error !== 'text-too-long') {
          DeviceTTS.chunks = [];
          return;
        }
        DeviceTTS.chunks.shift();
        DeviceTTS.trySpeak();
      }
      utterance.onend = finish;
      utterance.onerror = finish;
      synth.speak(utterance);
    }
  },

  SpeakOnDevice__deps: ['$DeviceTTS'],
  SpeakOnDevice: function (text, rateMultiplier) {
    if (!window.speechSynthesis || !window.SpeechSynthesisUtterance) return;
    DeviceTTS.listen();
    DeviceTTS.rate = rateMultiplier;
    var words = UTF8ToString(text).trim().split(/\s+/);
    var chunk = '';
    for (var i = 0; i < words.length; i++) {
      if (chunk && (chunk.length + words[i].length > 180 || /[.!?]$/.test(chunk))) {
        DeviceTTS.chunks.push(chunk);
        chunk = '';
      }
      chunk += (chunk ? ' ' : '') + words[i];
    }
    if (chunk) DeviceTTS.chunks.push(chunk);
    DeviceTTS.trySpeak();
  },

  StopDeviceSpeech__deps: ['$DeviceTTS'],
  StopDeviceSpeech: function () {
    var hadUtterance = DeviceTTS.utterance !== null;
    DeviceTTS.chunks = [];
    DeviceTTS.utterance = null;
    clearTimeout(DeviceTTS.restart);
    DeviceTTS.restart = null;
    if (hadUtterance) window.speechSynthesis.cancel();
  },

  TTSDeviceBusy__deps: ['$DeviceTTS'],
  TTSDeviceBusy: function () {
    return DeviceTTS.chunks.length > 0 ? 1 : 0;
  },

  SetDeviceVolume__deps: ['$DeviceTTS'],
  SetDeviceVolume: function (value) {
    if (DeviceTTS.volume === value) return;
    DeviceTTS.volume = value;
    if (!DeviceTTS.utterance && DeviceTTS.restart === null) return;

    if (DeviceTTS.utterance) {
      DeviceTTS.utterance = null;
      window.speechSynthesis.cancel();
    }
    clearTimeout(DeviceTTS.restart);
    DeviceTTS.restart = setTimeout(function () {
      DeviceTTS.restart = null;
      DeviceTTS.trySpeak();
    }, 150);
  }
});
