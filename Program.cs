/*
 * noisedetector (c) mjt 2022
 * 
 * released under MIT license.
 * 
 * starts program when mic volume goes over peak point.
 * check noise_config.txt
 * 
 * MicrophoneVolume() from here (thanks):
 *   https://stackoverflow.com/questions/20943469/how-to-get-microphone-volume-in-windows-phone-8
 * 
 * */
using System;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Noisedetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("noisedetector  (c) mjt 2022");

            MyGame mg = new MyGame();
            mg.Run();
        }
    }

    public class MyGame : Game
    {
        public MyGame()
        {
            GraphicsDeviceManager graphics;
            graphics = new GraphicsDeviceManager(this);
            graphics.ApplyChanges();

            new MicrophoneTest().Init();

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Console.WriteLine("...");
        }
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //Console.WriteLine("###");
        }
    }

    public class MicrophoneTest
    {
        Microphone mp;
        byte[] buffer;

        int mic = 0;
        int maxvolume = 0;
        int tests = 1;

        bool started = false;
        long startTime = 0, endTime = 0;
        int lengthInMinutesWhenToStop = 0;
        string programToStart = "";
        string programParameters = "";

        int vol = 0;
        int c = 0;

        int debug = 1;

        /// <summary>
        /// // https://stackoverflow.com/questions/20943469/how-to-get-microphone-volume-in-windows-phone-8
        /// Detecting volume changes, the RMS Method.
        /// RMS(Root mean square)
        public int MicrophoneVolume(byte[] data)
        {
            //RMS Method
            double rms = 0;
            ushort byte1 = 0;
            ushort byte2 = 0;
            short value = 0;
            rms = (short)(byte1 | (byte2 << 8));

            for (int i = 0; i < data.Length - 1; i += 2)
            {
                byte1 = data[i];
                byte2 = data[i + 1];

                value = (short)(byte1 | (byte2 << 8));
                rms += Math.Pow(value, 2);
            }

            rms /= (double)(data.Length / 2);
            return (int)Math.Floor(Math.Sqrt(rms));
        }

        public void OnBufferReady(object sender, EventArgs args)
        {
            mp.GetData(buffer);

            int v = MicrophoneVolume(buffer);
            vol += v;

            c++;
            if (c < tests) return;

            int avg = vol / tests;
            c = 0;
            vol = 0;

            if (debug == 1) Console.WriteLine("[DEBUG]: vol = " + avg);

            if (avg > maxvolume)
            {
                avg = 0;
                Console.WriteLine("    --- SOME NOISE --- ");
                startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                endTime = startTime + (lengthInMinutesWhenToStop * 60 * 10);

                if (started == false)
                {
                    started = true;
                    Console.WriteLine("starting " + programToStart + " at " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);

                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = programToStart;
                        process.StartInfo.Arguments = programParameters;
                        process.Start();
                        //process.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        //Console.ReadKey();
                        return;
                    }
                }
            }

            if (started)
            {
                long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                //if(debug == 1) Console.WriteLine("[DEBUG]:  ms=" + milliseconds + "   endTime=" + endTime);
                if (milliseconds >= endTime)
                {
                    Console.WriteLine("end " + programToStart);
                    started = false;

                    try
                    {
                        Process process = new Process();
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            process.StartInfo.FileName = "taskkill";
                            process.StartInfo.Arguments = "/im " + programToStart;
                        }
                        else
                        {
                            process.StartInfo.FileName = "pkill";
                            process.StartInfo.Arguments = programToStart;
                        }
                        process.Start();
                        //process.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        //Console.ReadKey();
                        return;
                    }

                }
            }
        }

        void LoadConfig()
        {
            string[] lines = File.ReadAllLines("noise_config.txt");

            // skip pre comments
            int st = 0;
            for (int q = 0; q < lines.Length; q++)
            {
                if (lines[q].Length == 0 || lines[q][0] == '#')
                    continue;
                else
                {
                    st = q;
                    break;
                }
            }

            // read config
            mic = int.Parse(lines[st++]);
            tests = int.Parse(lines[st++]);
            maxvolume = int.Parse(lines[st++]);
            programToStart = lines[st++];
            programParameters = lines[st++];
            lengthInMinutesWhenToStop = int.Parse(lines[st++]);
            debug = int.Parse(lines[st++]);
        }


        public void Init()
        {
            Console.WriteLine("Microphones found: " + Microphone.All.Count);

            if (Microphone.All.Count == 0)
            {
                Console.WriteLine("No microphone found. Exiting...");
                return;
            }

            Console.WriteLine("load noise_config.txt");
            LoadConfig();
            Console.WriteLine("ok");

            //mp = Microphone.Default;
            mp = Microphone.All[mic];

            int bufferSize = mp.GetSampleSizeInBytes(mp.BufferDuration);
            buffer = new byte[bufferSize];
            mp.BufferReady += new EventHandler<EventArgs>(OnBufferReady);

            if (debug == 1) Console.WriteLine("[DEBUG]: buffersize = " + bufferSize);

            mp.Start();
        }

        public void Close()
        {
            if (mp != null) mp.Stop();
            mp = null;
        }
    }
}
