using System;
using System.Collections.Generic;
using System.Text;

namespace Factotum
{
	class EpriGrid
	{
		private EpriGridItem[] items;

		public EpriGrid()
		{
			items = new EpriGridItem[] {
				new EpriGridItem(0,	0.5m, 0.5m),
				new EpriGridItem(2,	1m,	1m),
				new EpriGridItem(4,	1m,   1.17m),
				new EpriGridItem(6,	1m,	1.73m),
				new EpriGridItem(8,	2m,	2.25m),
				new EpriGridItem(10,	2m,	2.81m),
				new EpriGridItem(12,	3m,	3.33m),
				new EpriGridItem(14,	3m,	3.67m),
				new EpriGridItem(16,	3m,	4.19m),
				new EpriGridItem(18,	4m,	4.71m),
				new EpriGridItem(20,	4m,	5.23m),
				new EpriGridItem(24,	4m,	6m)
			};
		}
		public decimal GetRecommendedGridForDiameter(decimal diameter)
		{
			int lastItem = items.Length - 1;
			EpriGridItem item;
			for (int i = lastItem; i >= 0; i--)
			{
				item = items[i];
				if ((int)(Math.Floor(diameter)) >= item.Diameter)
					return item.RecommendedGrid;				
			}
			throw new Exception("Unable to Find an EPRI Recommended Grid for diameter " + diameter.ToString());
		}

		public decimal GetMaxGridForDiameter(decimal diameter)
		{
			int lastItem = items.Length - 1;
			EpriGridItem item;
			for (int i = lastItem; i >= 0; i--)
			{
				item = items[i];
				if ((int)(Math.Floor(diameter)) >= item.Diameter)
					return item.MaxGrid;
			}
			throw new Exception("Unable to Find an EPRI Maximum Grid for diameter " + diameter.ToString());
		}
	}
	class EpriGridItem
	{
		private int diameter;
		private decimal maxGrid;
		private decimal recommendedGrid;

		public int Diameter
		{
			get { return diameter;}
			set { diameter = value;}
		}

		public decimal MaxGrid
		{
			get { return maxGrid;}
			set { maxGrid = value;}
		}

		public decimal RecommendedGrid
		{
			get { return recommendedGrid;}
			set { recommendedGrid = value;}
		}

		public EpriGridItem(int diameter, decimal recommendedGrid, decimal maxGrid)
		{
			this.diameter = diameter;
			this.recommendedGrid = recommendedGrid;
			this.maxGrid = maxGrid;
		}

	}
}
