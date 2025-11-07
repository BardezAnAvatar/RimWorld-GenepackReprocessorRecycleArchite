using System;
using System.Collections.Generic;
using GenepackReprocessor;
using RimWorld;
using UnityEngine;
using Verse;

namespace Bardez.Biotech.GenepackReprocessor.Recycle
{
    public class Dialog_RecycleArchiteGenepack : GeneCreationDialogBase
    {
        protected Genepack SelectedGenepack => this.selectedGenepack;
        private Building_GeneSeparator geneSeparator;
        private List<Genepack> libraryGenepacks = new List<Genepack>();
        private List<Genepack> unpoweredGenepacks = new List<Genepack>();
        private List<Genepack> selectedGenepacks = new List<Genepack>();
        private HashSet<Genepack> matchingGenepacks = new HashSet<Genepack>();
        private readonly Color UnpoweredColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private List<GeneDef> tmpGenes = new List<GeneDef>();
        private Genepack selectedGenepack;

        /// <summary>Constructor</summary>
        /// <param name="geneSeparator">The specific <see cref="Building_GeneSeparator" /> that the dialog is rendering for</param>
        public Dialog_RecycleArchiteGenepack(Building_GeneSeparator geneSeparator)
        {
            this.geneSeparator = geneSeparator;
            this.libraryGenepacks.AddRange(geneSeparator.GetGenepacks(true, true));
            this.unpoweredGenepacks.AddRange(geneSeparator.GetGenepacks(false, true));
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            this.searchWidgetOffsetX = Convert.ToSingle((GeneCreationDialogBase.ButSize).x * 2.0 + 4.0);
            GeneUtility.SortGenepacks(this.libraryGenepacks);
            GeneUtility.SortGenepacks(this.unpoweredGenepacks);
        }

        protected override string Header => throw new NotImplementedException();

        protected override string AcceptButtonLabel => throw new NotImplementedException();

        public override string CloseButtonText => throw new NotImplementedException();

        protected override List<GeneDef> SelectedGenes => throw new NotImplementedException();

        protected override void Accept()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateSearchResults()
        {
            throw new NotImplementedException();
        }

        protected override void DrawGenes(Rect rect)
        {
            throw new NotImplementedException();
        }
    }
}
