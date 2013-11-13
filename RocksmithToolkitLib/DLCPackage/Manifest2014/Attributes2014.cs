﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RocksmithToolkitLib.Xml;
using RocksmithToolkitLib.DLCPackage.Manifest.Tone;
using RocksmithToolkitLib.DLCPackage.Manifest.Header;
using RocksmithToolkitLib.DLCPackage.AggregateGraph;
using RocksmithToolkitLib.Extensions;

namespace RocksmithToolkitLib.DLCPackage.Manifest
{
    public class Attributes2014 : AttributesHeader2014, IAttributes {
        private int[] songPartitionCount = { 0 /* Combo count */, 0 /* Lead count */, 0 /* Rhythm count */, 0 /* Bass Count */ }; 

        public SongArrangementProperties2014 ArrangementProperties { get; set; }
        public int ArrangementSort { get; set; }
        public int ArrangementType { get; set; }
        public string BlockAsset { get; set; }
        public Dictionary<string, Dictionary<string, object>> Chords { get; set; } //Problem in 3rd sublevel that can be a list or not
        public List<ChordTemplate> ChordTemplates { get; set; }
        public List<float> DynamicVisualDensity { get; set; }
        public string FullName { get; set; }
        public string LastConversionDateTime { get; set; }
        public int MasterID_PS3 { get; set; }
        public int MasterID_Xbox360 { get; set; }
        public int MaxPhraseDifficulty { get; set; }
        public List<PhraseIteration> PhraseIterations { get; set; }
        public List<Phrase> Phrases { get; set; }
        public string PreviewBankPath { get; set; }
        public int RelativeDifficulty { get; set; }
        public float Score_MaxNotes { get; set; }
        public float Score_PNV { get; set; }
        public List<Section> Sections { get; set; }
        public string ShowlightsXML { get; set; }
        public string SongAsset { get; set; }
        public float SongAverageTempo { get; set; }
        public string SongBank { get; set; }
        public string SongEvent { get; set; }
        public float SongOffset { get; set; }
        public int SongPartition { get; set; }
        public string SongXml { get; set; }
        public int TargetScore { get; set; }
        public Dictionary<string, Dictionary<string, object>> Techniques { get; set; } //Problem in 3rd sublevel that can be a list or not
        public string Tone_A { get; set; }
        public string Tone_B { get; set; }
        public string Tone_Base { get; set; }
        public string Tone_C { get; set; }
        public string Tone_D { get; set; }
        public string Tone_Multiplayer { get; set; }
        public List<ToneRS2014> Tones { get; set; }
        public string InputEvent { get; set; } //Vocals only

        public Attributes2014() {}

        public Attributes2014(Arrangement arrangement, DLCPackageData info, AggregateGraph2014 aggregateGraph, Platform platform)
            : base(arrangement, info, aggregateGraph, platform)
        {

            #region VARIABLES

            var dlcName = info.Name.ToLower();
            var arrangementName = arrangement.Name.ToString().ToLower();

            var xblockUrn = String.Format(URN_TEMPLATE, TagValue.EmergentWorld.GetDescription(), TagValue.XWorld.GetDescription(), aggregateGraph.GameXblock.Name);
            var showlightUrn = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.XML.GetDescription(), aggregateGraph.ShowlightXml.Name);
            var songXmlUrn = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.XML.GetDescription(), String.Format(AggregateGraph2014.NAME_DEFAULT, dlcName, arrangementName));
            var songSngUrn = String.Format(URN_TEMPLATE, TagValue.Application.GetDescription(), TagValue.MusicgameSong.GetDescription(), String.Format(AggregateGraph2014.NAME_DEFAULT, dlcName, arrangementName));

            var manifestFunctions = new ManifestFunctions(platform.version);

            #endregion

            #region FILL ATTRIBUTES

            ArrangementSort = arrangement.ArrangementSort;
            BlockAsset = xblockUrn;
            manifestFunctions.GenerateDynamicVisualDensity(this, SongContent, arrangement);
            FullName = String.Format(AggregateGraph2014.NAME_DEFAULT, info.Name, arrangement.Name);
            MasterID_PS3 = (IsVocal) ? -1 : arrangement.MasterId;
            MasterID_Xbox360 = (IsVocal) ? -1 : arrangement.MasterId;
            PreviewBankPath = String.Format(AggregateGraph2014.NAME_SOUNDBANKPREVIEW + ".bnk", info.Name.ToLower());
            RelativeDifficulty = (IsVocal) ? 0 : SongContent.Levels.Count();
            ShowlightsXML = showlightUrn;
            SongAsset = songSngUrn;
            SongBank = String.Format(AggregateGraph2014.NAME_SOUNDBANK + ".bnk", info.Name.ToLower());
            SongEvent = String.Format("Play_", info.Name);
            SongXml = songXmlUrn;
                
            // Only for Vocal
            if (IsVocal)
                InputEvent = "Play_Tone_Standard_Mic";

            // Only for instruments
            if (!IsVocal)
            {
                ArrangementProperties = SongContent.ArrangementProperties;
                ArrangementType = (int)arrangement.ArrangementTypeEnum;

                //Chords        -- //TODO: MISSING GENERATE

                ChordTemplates = new List<ChordTemplate>();
                manifestFunctions.GenerateChordTemplateData(this, SongContent);

                LastConversionDateTime = SongContent.LastConversionDateTime;
                MaxPhraseDifficulty = 0; //TODO: MISSING GENERATE

                PhraseIterations = new List<PhraseIteration>();
                manifestFunctions.GeneratePhraseIterationsData(this, SongContent);
                //Score_MaxNotes -- Generated on function above
                //Score_PNV      -- Generated on function above

                Phrases = new List<Phrase>();
                manifestFunctions.GeneratePhraseData(this, SongContent);
                
                Sections = new List<Section>();
                manifestFunctions.GenerateSectionData(this, SongContent);

                SongAverageTempo = SongContent.AverageTempo;
                SongOffset = 0; //TODO: ???

                if (arrangement.ArrangementType != Sng.ArrangementType.Vocal)
                    SongPartition = manifestFunctions.GetSongPartition(arrangement.Name, arrangement.ArrangementType);

                TargetScore = 0;
                //Techniques     -- //TODO: MISSING GENERATE
                Tone_A = arrangement.ToneA;
                Tone_B = arrangement.ToneB;
                Tone_Base = arrangement.ToneBase;
                Tone_C = arrangement.ToneC;
                Tone_D = arrangement.ToneD;
                Tone_Multiplayer = arrangement.ToneMultiplayer;
                //Tones
            }

            #endregion
        }
    }
}
