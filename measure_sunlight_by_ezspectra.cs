using System;
using OaktreeLab.ezSpectra;
using System.IO;
using System.Text;
using System.Threading;

namespace ezSpectraAPI_Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteRunNumber();

            DateTime tm_now;
            DateTime tm_last = DateTime.Now;
            int end_t = 18 * 60;
            int last_t = tm_last.Hour * 60 + 10 * (tm_last.Minute / 10);
            int now_t = last_t + 10;
            
            ezSpectraAPI ezs = new ezSpectraAPI();
            ezs.Connect();
            Thread.Sleep(1000);
            ezs.SamplingQuality = SamplingQualityType.None;
            ezs.AutoExposure = true;
            double et = ezs.ExposureTime;
//
            while (now_t <= end_t)
            {
                ezs.ExecuteSampling();
                Thread.Sleep(5000);
                tm_now = DateTime.Now;
                now_t = tm_now.Hour * 60 + tm_now.Minute;
                if (now_t - last_t >= 10)
                {
                    Spectrum s = ezs.CurrentRawSpectrum;
                    AnalysisResult ar = ezs.CurrentAnalysisResult;
                    WriteCsv(now_t, s, ar, et);
                    last_t = now_t;
                    Console.WriteLine("Capture... OK" + tm_now);
                }                
            }
///
            ezs.Reset();
            ezs.Close();
            Console.WriteLine("save " + now_t.ToString() + "saved...");
        }

        private static void WaitKey()

        {
            Console.WriteLine("Hit return key.");
            Console.ReadLine();
        }

        private static void WriteCsv(int nt, Spectrum s, AnalysisResult ar, double et)
        {
            StreamWriter file = new StreamWriter(("spectrumdata" + ".csv"), true, Encoding.UTF8);
            string lengthArray = string.Join(",", s.WaveLengthList);
            file.Write(nt);
            file.Write(",");
            //file.Write($"{lengthArray}");
            string valueArray = string.Join(",", s.Values);
            file.Write($"{valueArray}");
            string raArray = string.Join(",", ar.Ri);
            file.Write($"{raArray}");
            file.Write(",");
            file.Write($"{ar.CCT}");
            file.Write(",");
            file.Write($"{ar.x}");
            file.Write(",");
            file.Write($"{ar.y}");
            file.Write(",");
            file.Write($"{ar.u}");
            file.Write(",");
            file.Write($"{ar.v}");
            file.Write(",");
            file.Write($"{ar.Ra}");
            file.Write(",");
            file.Write($"{ar.Lux}");
            file.Write(",");
            file.Write($"{ar.PPFD}");
            file.Write(",");
            file.Write(et);
            file.WriteLine($"");
            file.Close();
        }
        private static void WriteRunNumber()
        {
            Console.Write("Run No.:");
            string name = Console.ReadLine();

            StreamWriter file = new StreamWriter("filename.txt", false);

            file.WriteLine(name);
            file.Close();
        }

    }
}
