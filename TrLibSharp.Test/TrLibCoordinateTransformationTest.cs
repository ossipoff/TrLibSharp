using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrLibSharp.Test
{
  [TestClass]
  public class TrLibCoordinateTransformationTest
  {
    [TestMethod]
    public void s34s_to_euref89_transformation_correct_for_smallest_possible_values()
    {
      var coordinateTransformation = new TrLibCoordinateTransformation();

      var from = new Point()
      {
        X = 31610,
        Y = 78692,
        EpsgId = 34005
      };

      var to = coordinateTransformation.Transform(from, 25832);

      Assert.AreEqual(766008.927, Math.Truncate(1000 * to.X) / 1000);
      Assert.AreEqual(6114767.329, Math.Truncate(1000 * to.Y) / 1000);
      Assert.AreEqual(25832, to.EpsgId);
    }

    [TestMethod]
    public void s34s_to_euref89_transformation_correct_for_Amalienborg()
    {
      var coordinateTransformation = new TrLibCoordinateTransformation();

      var from = new Point()
      {
        X = 70522.9080,
        Y = 141451.6275,
        EpsgId = 34005
      };

      var to = coordinateTransformation.Transform(from, 25832);

      Assert.AreEqual(725859.521, Math.Truncate(1000 * to.X) / 1000);
      Assert.AreEqual(6176769.440, Math.Truncate(1000 * to.Y) / 1000);
      Assert.AreEqual(25832, to.EpsgId);

    }
  }
}
