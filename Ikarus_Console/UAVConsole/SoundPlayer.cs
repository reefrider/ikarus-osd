using System;
using System.Collections.Generic;
using Microsoft.DirectX.AudioVideoPlayback;
using Microsoft.DirectX.DirectSound;
using System.Threading;
using Microsoft.DirectX;

namespace Totea
{
    class SoundPlayer
    {
        private const int SAMPLES = 256;
        //        private const int MAX_IN_VOLUME = 17000; //32767 is the max allowed
        private static int MAX_IN_VOLUME = 6000; //32767 is the max allowed
        private const int MIN_IN_VOLUME = 200; //0 is the min allowed
        private const int MAX_OUT_VOLUME = 0; //0 is the max allowed
        private const int MIN_OUT_VOLUME = -2000; //-10000 is the min allowed
        private static int[] SAMPLE_FORMAT_ARRAY = { SAMPLES, 1, 1 };

        private static Queue<String> queueLowPriority = new Queue<string>();
        private static Queue<String> queueMediumPriority = new Queue<string>();
        private static Queue<String> queueHighPriority = new Queue<string>();
        private static String[] currentSounds = new String[3];

        private static Audio backMusic;
        private static Audio soundHP;
        private static Audio soundMP;
        private static Audio soundAlarm = new Audio("AlarmaTotea.mp3");


        private static Thread queueSoundThread;
        private static CaptureBuffer buffer;

        public static bool alarmEnable;

        public static void Inicialize()
        {
            for (int i = 0; i < 3; i++)
                currentSounds[i] = "";

            alarmEnable = false;
            //MAX_IN_VOLUME = ToteaConfigManager.getMainConfig().overheadMicrophone;

            StartSoundThread();
        }

        public static void Stop()
        {
            StopSoundThread();

            if (backMusic != null)
            {
                backMusic.Stop();
                backMusic.Dispose();
                backMusic = null;
            }

            if (soundHP != null)
            {
                soundHP.Stop();
                soundHP.Dispose();
                soundHP = null;
            }

            if (soundMP != null)
            {
                soundMP.Stop();
                soundMP.Dispose();
                soundMP = null;
            }

            queueLowPriority.Clear();
            queueMediumPriority.Clear();
            queueHighPriority.Clear();

            for (int i = 0; i < 3; i++)
                currentSounds[i] = "";
        }
        /*
        public static bool EnqueueSound(string fileName, ToteaAudioMenssageConfig.Priority priority)
        {
            bool allowed = !queueLowPriority.Contains(fileName) &&
                        !queueMediumPriority.Contains(fileName) &&
                        !queueHighPriority.Contains(fileName) &&
                        !currentSounds.Contains<String>(fileName);
            if (allowed)
            {
                switch (priority)
                {
                    case ToteaAudioMenssageConfig.Priority.Low:
                        queueLowPriority.Enqueue(fileName);
                        currentSounds[0] = fileName;
                        break;
                    case ToteaAudioMenssageConfig.Priority.Medium:
                        queueMediumPriority.Enqueue(fileName);
                        currentSounds[1] = fileName;
                        break;
                    case ToteaAudioMenssageConfig.Priority.High:
                        queueHighPriority.Enqueue(fileName);
                        currentSounds[2] = fileName;
                        break;
                }
                Console.WriteLine(fileName + "\t" + priority + "\t(" + DateTime.Now.ToLongTimeString() + ")");
            }

            return allowed;
        }
        */
        static void StartSoundThread()
        {
            StopSoundThread();

            InicialiceCaptureBuffer();

            queueSoundThread = new Thread(new ThreadStart(UpdateQueues));
            queueSoundThread.Priority = ThreadPriority.Lowest;
            queueSoundThread.Start();
        }

        static void StopSoundThread()
        {
            DisposeCaptureBuffer();

            if (queueSoundThread != null)
            {
                queueSoundThread.Abort();
                queueSoundThread.Join();
                queueSoundThread = null;
            }
        }

        static void InicialiceCaptureBuffer()
        {
            try
            {
                CaptureDevicesCollection audioDevices = new CaptureDevicesCollection();

                // initialize the capture buffer and start the animation thread
                Capture cap = new Capture(audioDevices[1].DriverGuid);
                CaptureBufferDescription desc = new CaptureBufferDescription();
                WaveFormat wf = new WaveFormat();
                wf.BitsPerSample = 16;
                wf.SamplesPerSecond = 44100;
                wf.Channels = (short)cap.Caps.Channels;
                wf.BlockAlign = (short)(wf.Channels * wf.BitsPerSample / 8);
                wf.AverageBytesPerSecond = wf.BlockAlign * wf.SamplesPerSecond;
                wf.FormatTag = WaveFormatTag.Pcm;

                desc.Format = wf;
                desc.BufferBytes = SAMPLES * wf.BlockAlign;

                buffer = new CaptureBuffer(desc, cap);
                buffer.Start(true);
            }
            catch
            {
                Console.WriteLine("Error al iniciar el capturador de sonido");
            }
        }

        static void DisposeCaptureBuffer()
        {
            if (buffer != null)
            {
                if (buffer.Capturing)
                {
                    buffer.Stop();
                }

                buffer.Dispose();
                buffer = null;
            }
        }

        static void UpdateQueues()
        {
            bool fromAlarm = false;
            bool configVolumeChanged = false;
            int baseVolume = 1;
            int finalVolume = 0;
            //ToteaMainConfig main_config = ToteaConfigManager.getMainConfig();
            int configVolume = -1;
            DateTime timeFlag = new DateTime();

            while (true)
            {
                string newSound = "";

                try
                {
                    if (alarmEnable)
                    {
                        if (!fromAlarm)
                        {
                            if (backMusic != null)
                                backMusic.Pause();

                            if (soundHP != null)
                                soundHP.Pause();

                            if (soundMP != null)
                                soundMP.Pause();

                            fromAlarm = true;
                            timeFlag = DateTime.Now;
                        }

                        if ((!soundAlarm.Playing || soundAlarm.CurrentPosition >= soundAlarm.Duration) && (DateTime.Now - timeFlag).Minutes < 10)
                        {
                            soundAlarm.CurrentPosition = 0;
                            soundAlarm.Play();
                        }
                    }
                    else
                    {
                        if (fromAlarm)
                        {
                            soundAlarm.Stop();
                            if (backMusic != null && backMusic.CurrentPosition < backMusic.Duration)
                                backMusic.Play();
                        }

                        //Silece mode -> calculate new volume
                        if (buffer != null && (finalVolume == -10000 || (currentSounds[0] == "" && currentSounds[1] == "" && currentSounds[2] == "")))
                        {
                            Array samples = buffer.Read(0, typeof(Int16), LockFlag.FromWriteCursor, SAMPLE_FORMAT_ARRAY);

                            int goal = 0;

                            // average across all samples to get the goals
                            for (int i = 0; i < SAMPLES; i++)
                            {
                                goal += Math.Abs(Convert.ToInt32(samples.GetValue(i, 0, 0)));
                            }
                            goal = goal / SAMPLES;

                            //Normalize
                            goal = Math.Max(Math.Min(goal, MAX_IN_VOLUME), MIN_IN_VOLUME);
                            goal = (int)map(goal, MIN_IN_VOLUME, MAX_IN_VOLUME, MIN_OUT_VOLUME, MAX_OUT_VOLUME);

                            if (baseVolume <= 0)
                                baseVolume = (int)((float)baseVolume * 0.8f + (float)goal * 0.2f);
                            else
                                baseVolume = goal;

                            configVolume = -1;
                        }

                        //ConfigVolume changed
                        /*
                        if (configVolumeChanged = (configVolume != (configVolume = main_config.currentVolume)))
                        {
                            if (configVolume != 0)
                                finalVolume = (int)map(configVolume, 0, 100, baseVolume, MAX_OUT_VOLUME);
                            else
                                finalVolume = -10000;//Silence

                            Console.Title = String.Format("{1}% de {0} = {2}", baseVolume, configVolume, finalVolume);
                        }
                        */
                        //Background music
                        if (backMusic == null || backMusic.CurrentPosition >= backMusic.Duration)
                        {
                            if (backMusic != null)
                            {
                                backMusic.Dispose();
                                backMusic = null;
                            }

                            if (queueLowPriority.Count > 0)
                            {
                                newSound = queueLowPriority.Dequeue();
                                currentSounds[0] = newSound;
                                backMusic = new Audio("Audio/" + newSound);
                                backMusic.Volume = finalVolume;

                                backMusic.Play();
                            }
                            else
                            {
                                currentSounds[0] = "";
                            }
                        }
                        else if (configVolumeChanged)
                        {
                            backMusic.Volume = finalVolume;
                        }

                        //Priority queues
                        //Check high priority queue
                        if (soundHP != null && soundHP.CurrentPosition < soundHP.Duration)
                        {
                            if (!soundHP.Playing)
                                soundHP.Play();

                            if (configVolumeChanged)
                                soundHP.Volume = finalVolume;

                            goto END;
                        }
                        else if (queueHighPriority.Count > 0)
                        {
                            if (backMusic != null)
                                backMusic.Volume -= 1000;

                            if (soundMP != null)
                                soundMP.Pause();

                            newSound = queueHighPriority.Dequeue();
                            currentSounds[2] = newSound;

                            soundHP = new Audio("Audio/" + newSound);
                            soundHP.Play();
                            soundHP.Volume = finalVolume;

                            //ATCHIDManager.soundPlaying();

                            goto END;
                        }
                        else
                        {
                            //High priority sound finished
                            if (currentSounds[2] != "")
                            {
                                soundHP.Dispose();
                                soundHP = null;
                                currentSounds[2] = "";

                                if (backMusic != null)
                                    backMusic.Volume += 1000;
                            }

                            //Check medium priority queue
                            if (soundMP != null && soundMP.CurrentPosition < soundMP.Duration)
                            {
                                if (!soundMP.Playing)
                                    soundMP.Play();

                                if (configVolumeChanged)
                                    soundMP.Volume = finalVolume;

                                goto END;
                            }
                            else if (queueMediumPriority.Count > 0)
                            {
                                if (backMusic != null)
                                    backMusic.Volume -= 1000;

                                newSound = queueMediumPriority.Dequeue();
                                currentSounds[1] = newSound;

                                soundMP = new Audio("Audio/" + newSound);
                                soundMP.Play();
                                soundMP.Volume = finalVolume;

                                //ATCHIDManager.soundPlaying();

                                goto END;
                            }
                            else
                            {
                                //Medium priority sound finished
                                if (currentSounds[1] != "")
                                {
                                    soundMP.Dispose();
                                    soundMP = null;
                                    currentSounds[1] = "";

                                    if (backMusic != null)
                                        backMusic.Volume += 1000;
                                }
                            }
                        }

                    END:
                        fromAlarm = false;
                    }
                }
                catch (Exception e)
                {
                    if (e is DirectXException)
                        Console.WriteLine("Error al cargar el archivo de sonido: " + newSound);
                }
                Thread.Sleep(200);
            }
        }

        static float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }

}
