﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AudioLib.SplineLut
{
	public class SplineInterpolator
	{
        public class Splines
        {
            public static string JfetStageLinear = @"-2.4999030916549194,-1.7674854385964911,-1.6253506825876813,-1.5339927771936688,-1.4729481405773122,-1.4122338566894339,-1.3487496381956967,-0.90009221271966711,0.19962275158087306,0.37059064781344975,0.41636457578814084,0.46580982151223177,0.54799056228357812,0.75141519917044175,1.54970767184074,1.7004239596822206   ;   19.672,19.672,19.66990348070912,19.652894665434683,19.5359105340637,19.291849423096195,18.891223186383691,15.052670564270212,4.1471571534023557,2.4210090173133634,2.0919277579033282,1.8554054572673131,1.7178535461367452,1.6377858832655965,1.554,1.554   ;   0.0,0.0,0.0,-0.56362668975281549,-2.99889479068658,-5.4397741084427729,-6.7215363511659856,-9.3211329447565667,-9.6914732550368168,-8.8385682980277558,-6.1131201271341409,-3.5792549306062793,-0.69027986945904818,-0.062625187750312825,0.0,0.0";
            public static string JfetStageHighGain = @"-1.9929658301819306,-1.2476803292848013,-0.515101246452722,-0.48919138896583625,-0.46084360542830682,-0.43762483207537756,-0.419545550119196,-0.37034433462146166,-0.16316989929801182,-0.037040229317106876,-0.026771453336301668,-0.012540761846906563,0.013346330943467585,0.12704592245815555,0.26972892731222337,0.86992478520752514,2.0098196670368833   ;   14.457,14.457,14.455939460854182,14.448985134152318,14.355255880837172,14.199849051452105,14.019637581374752,13.344074283572798,7.3745978113793251,1.8891673945495804,1.4537310846652372,1.3822230869762577,1.3268665579862708,1.2282933413643251,1.1781561993445984,1.09,1.09   ;   0.0,0.0,0.0,-1.5159303447591725,-5.1632212793373959,-8.60209558908258,-11.111111111111114,-18.098679259840416,-40.403266128991845,-54.130707283860438,-9.443778112446779,-3.2105178251324373,-1.5656296937578207,-0.5339674008342673,-0.22134246358470566,0.0,0.0";
            public static string ZenerClipper = @"-9.398929449545415,-7.9945847547916973,-4.001411361187535,-0.041387121116021539,4.05905658552162,7.9923185911465282,12.256772735529552   ;   -4.0424593477961164,-4.0168421202223143,-3.3695190568952422,-0.038042058766997476,3.3920967457848659,4.0104694414655677,4.1132738222608625   ;   0.0,0.073046018991964889,0.32555077600122623,0.94278462646830952,0.32555077600122623,0.053005958911864777,0.0";
            public static string D1N914Clipper = @"-11.92876297296054,-7.9891391168010966,-1.4811025709312014,-0.7081535605041932,-0.21418063689620054,0.52772395496592028,1.1154141751797377,3.3186095107730171,7.9966744877839853,10.15950873895731   ;   -0.68727886215457579,-0.67766459383185929,-0.580964089609628,-0.51207665124988844,-0.23156585781066089,0.46578004319844568,0.56007100052876868,0.632169338001374,0.67816987691562369,0.68071606629320613   ;   0.0,0.004909814719624526,0.036172308444580692,0.13717421124828522,0.86663239816392912,0.32555077600122623,0.062625187750312825,0.01693385076768459,0.004909814719624526,0.0";
            public static string LedClipper = @"-20.007141638482718,-2.915444438229212,-2.3941616350616366,-1.8681638548866069,-1.4632915735272494,1.2229924586554555,1.572843491744981,2.0432013992943152,2.8757611034829695,8.2512655010381142,20.015064279299438   ;   -1.731330241713267,-1.6225464128179352,-1.5972687114719562,-1.5391672822166931,-1.3508784208741995,1.1901803870295045,1.4297180755143659,1.5585133996818914,1.6157252109612972,1.6836420515583583,1.7312785241335889   ;   0.0,0.01693385076768459,0.053005958911864777,0.18527035544052548,0.69027986945904818,0.79368657947236787,0.59408758107456761,0.13717421124828522,0.028957886815744655,0.004909814719624526,0.0";
        }

        public double Bias;

		private double[] xs;
		private double[] ys;
		private double[] ks;
        private double max;
        private double min;

        public SplineInterpolator(string dataString, bool isJson = true)
        {
            if (isJson)
            {
                JsonUtil.Json.Value data = JsonUtil.Json.Parser.Parse(dataString);

                List<double> data0 = new List<double>(data[0].Count);
                List<double> data1 = new List<double>(data[1].Count);
                List<double> data2 = new List<double>(data[2].Count);

                foreach(JsonUtil.Json.Value value in data[0])
                {
                    data0.Add((double)value);
                }
                foreach (JsonUtil.Json.Value value in data[1])
                {
                    data1.Add((double)value);
                }
                foreach (JsonUtil.Json.Value value in data[2])
                {
                    data2.Add((double)value);
                }

                SetData(data0.ToArray(), data1.ToArray(), data2.ToArray());
            }
            else
            {
                var bits = dataString.Split(';');
                var d0 = bits[0].Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                var d1 = bits[1].Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                var d2 = bits[2].Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                SetData(d0, d1, d2);
            }
        }

        /// <summary>
        /// Arrays must be sorted from lowest x value to highest
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="ks"></param>
        public SplineInterpolator(double[] xs, double[] ys, double[] ks)
		{
            SetData(xs, ys, ks);
		}

        private void SetData(double[] xs, double[] ys, double[] ks)
        {
            this.xs = xs;
            this.ys = ys;
            this.ks = ks;
            this.max = xs.Max();
            this.min = xs.Min();
        }

        public void ProcessInPlace(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                var x = inputs[i] + Bias;
                if (x > max) x = max - 0.00001;
                if (x < min) x = min + 0.00001;
                var y = InterpolateNoBias(x);
                inputs[i] = y;
            }
        }

        public double Process(double input)
        {
            var x = input + Bias;
            if (x > max) x = max - 0.00001;
            if (x < min) x = min + 0.00001;
            var y = InterpolateNoBias(x);
            return y;
        }

        public double InterpolateNoBias(double x)
		{
			var i = 1;
			while (xs[i] < x)
				i++;
			
			var t = (x - xs[i - 1]) / (xs[i] - xs[i - 1]);
			
			var a = ks[i - 1] * (xs[i] - xs[i - 1]) - (ys[i] - ys[i - 1]);
			var b = -ks[i] * (xs[i] - xs[i - 1]) + (ys[i] - ys[i - 1]);

			var q = (1 - t) * ys[i - 1] + t * ys[i] + t * (1 - t) * (a * (1 - t) + b * t);
			return q;
		}
	}
}
