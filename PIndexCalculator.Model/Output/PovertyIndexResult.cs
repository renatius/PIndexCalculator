using System;
using FileHelpers;

namespace PICalculator.Model.Output
{

    [DelimitedRecord(";")]
    public class PovertyIndexResult
    {
        private int mWaveCount;

        public int WaveCount {
            get { return mWaveCount; }
            set { mWaveCount = value; }
        }
        
        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private String mCountry;

        public String Country {
            get { return mCountry; }
            set { mCountry = value; }
        }

        private String mPersonId;

        public String PersonId {
            get { return mPersonId; }
            set { mPersonId = value; }
        }

        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private String mPovertyYears;

        public String PovertySequence {
            get { return mPovertyYears; }
            set { mPovertyYears = value; }
        }

        [FieldQuoted('"', QuoteMode.AlwaysQuoted, MultilineMode.AllowForRead)]
        private String mPovertyGaps;

        public String PovertyGapSequence {
            get { return mPovertyGaps; }
            set { mPovertyGaps = value; }
        }

        private int mMaxSpell;

        public int MaxSpell {
            get { return mMaxSpell; }
            set { mMaxSpell = value; }
        }

        private double mPovertyGapAverage;

        public double PovertyGapAverage {
            get { return mPovertyGapAverage; }
            set { mPovertyGapAverage = value; }
        }

        private double mSequenceEffect1;

        public double SequenceEffect1 {
            get { return mSequenceEffect1; }
            set { mSequenceEffect1 = value; }
        }


        private double mSequenceEffect2;

        public double SequenceEffect2 {
            get { return mSequenceEffect2; }
            set { mSequenceEffect2 = value; }
        }

        private double mSequenceEffect3;

        public double SequenceEffect3 {
            get { return mSequenceEffect3; }
            set { mSequenceEffect3 = value; }
        }

        private double mSequenceEffect4;

        public double SequenceEffect4 {
            get { return mSequenceEffect4; }
            set { mSequenceEffect4 = value; }
        }

        private double mSequenceEffect5;

        public double SequenceEffect5 {
            get { return mSequenceEffect5; }
            set { mSequenceEffect5 = value; }
        }

        private double mEmergencyEffect;

        public double EmergencyEffect {
            get { return mEmergencyEffect; }
            set { mEmergencyEffect = value; }
        }

        private double mBossertIndex;

        public double BossertIndex
        {
            get  { return mBossertIndex; }
            set  { mBossertIndex = value; }
        }

        private double mBCD2;
        public double BCD2
        {
            get { return mBCD2; }
            set { mBCD2 = value; }
        }

        private double mAlpha;

        public double Alpha {
            get { return mAlpha; }
            set { mAlpha = value; }
        }

        private double mValue1;

        public double SE_EE_1 {
            get { return mValue1; }
            set { mValue1 = value; }
        }

        private double mValue2;

        public double SE_EE_2 {
            get { return mValue2; }
            set { mValue2 = value; }
        }

        private double mValue3;

        public double SE_EE_3 {
            get { return mValue3; }
            set { mValue3 = value; }
        }

        private double mValue4;

        public double SE_EE_4 {
            get { return mValue4; }
            set { mValue4 = value; }
        }

        private double mValue5;

        public double SE_EE_5 {
            get { return mValue5; }
            set { mValue5 = value; }
        }

        private double mDecayFactor;

        public double DecayFactor {
            get { return mDecayFactor; }
            set { mDecayFactor = value; }
        }
    }
}
