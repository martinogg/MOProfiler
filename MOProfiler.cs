// MO Profiler class
// MARTIN OGG 2013-2-7
// MOPROFILER.CS - A class to record timings
// usage: 	MOProfiler.StartProfileTime(1); // You can have as many numbers as you want
//  		... some code that takes time
//		MOProfiler.StartProfileTime(21);
//		... some code that takes time			
//		MOProfiler.EndProfileTime(1);
//		MOProfiler.EndProfileTime(21);
//		and in your update loop:
//		MOProfiler.updateShow();
// Every 100 update calls, an output is given to the System.Console; 

using System;
using System.Collections.Generic;

namespace YourNamespaceHere
{
	static class MOProfiler
	{
		
		static public Dictionary<int, long> StartTimings = new Dictionary<int, long>();
		static public Dictionary<int, long> FinalTimings = new Dictionary<int, long>();
		static public Dictionary<int, long> MaxTimings = new Dictionary<int, long>();
		static public Dictionary<int, int> FinalTimingCounts = new Dictionary<int, int>();

		static private int _framesPerShow = 100;
		static private int _currentFrameSkip = 101;
		
		public static long Now()
		{
			return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
		}
		
		public static void StartProfileTime(int index)
		{
			if (StartTimings.ContainsKey(index))
			{
				StartTimings.Remove(index);	
			}
			
			StartTimings.Add(index, Now());

		}
		
		public static void EndProfileTime(int index)
		{
			// time taken accumulation
			long acc = (long)0.0;
			if (FinalTimings.ContainsKey(index))
			{
				acc = FinalTimings[index];
				FinalTimings.Remove(index);
			}
			
			long TimeTaken = Now() - StartTimings[index];
			acc += TimeTaken; 
			
			FinalTimings.Add(index, acc);
			
			// counts
			int iAcc = 0;
			if (FinalTimingCounts.ContainsKey(index))
			{
				iAcc = FinalTimingCounts[index];
				FinalTimingCounts.Remove(index);
			}
			iAcc++;
			FinalTimingCounts.Add(index, iAcc);
			
			// max time
			long prevMaxTime = (long)-1.0;
			if (MaxTimings.ContainsKey(index))
			{
				prevMaxTime = MaxTimings[index];
				MaxTimings.Remove(index);
			}
			if (prevMaxTime < TimeTaken)
			{
				prevMaxTime = TimeTaken;
			}

			MaxTimings.Add(index, prevMaxTime);	

		}
		
		public static void clearTimings()
		{
				StartTimings.Clear();
				FinalTimingCounts.Clear();
				FinalTimings.Clear();
				MaxTimings.Clear();
		}
		
		public static void updateShow()
		{
			_currentFrameSkip++;
			if (_currentFrameSkip >= _framesPerShow)
			{
				_currentFrameSkip = 0; // only show _framesPerShow times
				long now = Now();
				
				System.Console.WriteLine("PROFILER WRITES");
				
				List<int> keyList = new List<int>(FinalTimings.Keys);
				
				for (int i = 0; i < keyList.Count; i++)
				{
					int iKeyNum = keyList[i];
					long TotalTime = FinalTimings[iKeyNum];
					int TimingCounts = FinalTimingCounts[iKeyNum];
					long AvgTime = TotalTime / (long)TimingCounts;
					long MaxTime = MaxTimings[iKeyNum];
					System.Console.WriteLine("ID "+iKeyNum+", totalTime="+TotalTime+" hits="+TimingCounts+" AVGTIME="+AvgTime+" MAX="+MaxTime);
					
				}
				
				// now wipe the lists for next frame
				clearTimings();
				
			}
		}
	}
}

