namespace YggdrasilDataset.Model
{
    using System;
    using FileHelpers;
    using YggdrasilDataset.Discriminators;

    [IgnoreFirst]
    [DelimitedRecord(";")]
    public class PliRow
    {
        [FieldHidden]
        private int? _maxReports;

        [FieldConverter(ConverterKind.Date, "yyyy")]
        public DateTime Year { get; set; }

        public string Module { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double? A1 { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double? A2 { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double? A3 { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double? A4 { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double? A5 { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double? A6 { get; set; }

        [FieldConverter(ConverterKind.Double, ",")]
        public double MaxExam { get; set; }

        public Outcome Result { get; set; }

        [FieldHidden]
        public int MaxReports => _maxReports ?? (_maxReports =
                                     Convert.ToInt32(A1.HasValue)
                                     + Convert.ToInt32(A2.HasValue)
                                     + Convert.ToInt32(A3.HasValue)
                                     + Convert.ToInt32(A4.HasValue)
                                     + Convert.ToInt32(A5.HasValue)
                                     + Convert.ToInt32(A6.HasValue)).Value;
    }
}
