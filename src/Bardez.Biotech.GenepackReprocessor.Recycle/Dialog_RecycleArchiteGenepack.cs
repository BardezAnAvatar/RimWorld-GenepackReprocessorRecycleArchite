using System;
using System.Collections.Generic;
using System.Linq;
using GenepackReprocessor;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

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

        protected override string Header => "Bardez.Biotech.GenepackReprocessor.Recycle_Header".Translate();

        protected override string AcceptButtonLabel => "Bardez.Biotech.GenepackReprocessor.Recycle_Accept".Translate();

        public override string CloseButtonText => "Bardez.Biotech.GenepackReprocessor.Recycle_Close".Translate();

        protected override List<GeneDef> SelectedGenes
        {
            get
            {
                this.tmpGenes.Clear();
                foreach (GeneSetHolderBase selectedGenepack in this.selectedGenepacks)
                {
                    foreach (GeneDef geneDef in selectedGenepack.GeneSet.GenesListForReading)
                        this.tmpGenes.Add(geneDef);
                }
                return this.tmpGenes;
            }
        }

        protected override void Accept()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateSearchResults()
        {
            throw new NotImplementedException();
        }

        #region Drawing
        protected override void DrawGenes(Rect rect)
        {
            GUI.BeginGroup(rect);
            Rect rect1 = new Rect(0.0f, 0.0f, rect.width - 16f, this.scrollHeight);
            float curY1 = 0.0f;

            Widgets.BeginScrollView(GenUI.AtZero(rect), ref this.scrollPosition, rect1, true);
            Rect containingRect = rect1;
            containingRect.y = this.scrollPosition.y;
            containingRect.height = rect.height;

            this.DrawSection(rect, this.selectedGenepacks, "SelectedGenepacks".Translate(), ref curY1, ref this.selectedHeight, false, containingRect);
            float curY2 = curY1 + 8f;

            this.DrawSection(rect, this.libraryGenepacks, "GenepackLibrary".Translate(), ref curY2, ref this.unselectedHeight, true, containingRect);

            if (Event.current.type == EventType.Layout)
            {
                this.scrollHeight = curY2;
            }

            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawSection(
            Rect rect,
            List<Genepack> genepacks,
            string label,
            ref float curY,
            ref float sectionHeight,
            bool adding,
            Rect containingRect)
        {
            float curX = 4f;
            Rect rect1 = new Rect(10f, curY, rect.width - 16.0f - 10.0f, Text.LineHeight);

            Widgets.Label(rect1, label);
            if (!adding)
            {
                Text.Anchor = TextAnchor.UpperRight;
                GUI.color = ColoredText.SubtleGrayColor;
                Widgets.Label(rect1, "ClickToAddOrRemove".Translate());
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            curY += Text.LineHeight + 3f;
            float num = curY;
            Rect rect2 = new Rect(0.0f, curY, rect.width, sectionHeight);
            Widgets.DrawRectFast(rect2, Widgets.MenuSectionBGFillColor, null);
            curY += 4f;

            if (!GenCollection.Any<Genepack>(genepacks))
            {
                Text.Anchor = (TextAnchor)4;
                GUI.color = (Color)ColoredText.SubtleGrayColor;
                Widgets.Label(rect2, "(" + "NoneLower".Translate() + ")");
                GUI.color = Color.white;
                Text.Anchor = (TextAnchor)0;
            }
            else
            {
                for (int index = 0; index < genepacks.Count; ++index)
                {
                    Genepack genepack = genepacks[index];
                    if (!this.quickSearchWidget.filter.Active || this.matchingGenepacks.Contains(genepack) && (!adding || !this.selectedGenepacks.Contains(genepack)))
                    {
                        float packWidth = 34.0f
                            + (GeneCreationDialogBase.GeneSize.x * genepack.GeneSet.GenesListForReading.Count)
                            + 4.0f * (genepack.GeneSet.GenesListForReading.Count + 2);

                        if (curX + packWidth > rect.width - 16.0f)
                        {
                            curX = 4f;
                            curY += GeneCreationDialogBase.GeneSize.y + 8.0f + 14.0f;
                        }
                        if (adding && this.selectedGenepacks.Contains(genepack))
                        {
                            Widgets.DrawLightHighlight(new Rect(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8.0f));
                            curX += packWidth + 14f;
                        }
                        else if (this.DrawGenepack(genepack, ref curX, curY, packWidth, containingRect))
                        {
                            if (adding)
                            {
                                SoundStarter.PlayOneShotOnCamera((SoundDef)SoundDefOf.Tick_High, null);
                                this.selectedGenepacks.Clear();
                                this.selectedGenepacks.Add(genepack);
                                this.selectedGenepack = genepack;
                            }
                            else
                            {
                                SoundStarter.PlayOneShotOnCamera((SoundDef)SoundDefOf.Tick_Low, null);
                                this.selectedGenepacks.Remove(genepack);
                            }
                            this.OnGenesChanged();
                            break;
                        }
                    }
                }
            }

            curY += GeneCreationDialogBase.GeneSize.y + 12.0f;
            if (Event.current.type != EventType.Layout)
                return;

            sectionHeight = curY - num;
        }

        protected bool DrawGenepack(
            Genepack genepack,
            ref float curX,
            float curY,
            float packWidth,
            Rect containingRect)
        {
            bool result = false;

            if (genepack.GeneSet == null || GenList.NullOrEmpty(genepack.GeneSet.GenesListForReading))
                return false;

            Rect rect1 = new(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8.0f);
            if (!containingRect.Overlaps(rect1))
            {
                curX = rect1.xMax + 14f;
                return false;
            }

            Widgets.DrawHighlight(rect1);
            GUI.color = GeneCreationDialogBase.OutlineColorUnselected;
            Widgets.DrawBox(rect1, 1, null);
            GUI.color = Color.white;
            curX += 4f;
            GeneUIUtility.DrawBiostats(genepack.GeneSet.ComplexityTotal, genepack.GeneSet.MetabolismTotal, genepack.GeneSet.ArchitesTotal, ref curX, curY, 4f);
            List<GeneDef> genesListForReading = genepack.GeneSet.GenesListForReading;
            for (int index = 0; index < genesListForReading.Count; ++index)
            {
                GeneDef gene = genesListForReading[index];
                if (this.quickSearchWidget.filter.Active && this.matchingGenes.Contains(gene))
                    this.matchingGenepacks.Contains(genepack);

                bool flag2 = this.leftChosenGroups.Any(x => (x.overriddenGenes).Contains(gene));
                Rect rect2 = new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);

                string extraTooltip = null;
                if (this.leftChosenGroups.Any(x => x.leftChosen == gene))
                {
                    extraTooltip = GroupInfo(this.leftChosenGroups.FirstOrDefault(x => x.leftChosen == gene));
                }
                else if (this.cachedOverriddenGenes.Contains(gene))
                {
                    extraTooltip = GroupInfo(this.leftChosenGroups.FirstOrDefault(x => x.overriddenGenes.Contains(gene)));
                }
                else if (((Dictionary<GeneDef, List<GeneDef>>)this.randomChosenGroups).ContainsKey(gene))
                {
                    TaggedString randomGene = "GeneWillBeRandomChosen".Translate()
                        + ":\n"
                        + GenText.ToLineList(this.randomChosenGroups[gene].Select(x => x.label), "  - ", true);
                    extraTooltip = ColoredText.Colorize(randomGene, ColoredText.TipSectionTitleColor);
                }
                GeneUIUtility.DrawGeneDef(genesListForReading[index], rect2, (GeneType)1, (Func<string>)(() => extraTooltip), false, false, flag2);
                curX += GeneCreationDialogBase.GeneSize.x + 4.0f;
            }
            Widgets.InfoCardButton(rect1.xMax - 24f, rect1.y + 2f, genepack);
            if (this.unpoweredGenepacks.Contains(genepack))
            {
                Widgets.DrawBoxSolid(rect1, this.UnpoweredColor);
                TooltipHandler.TipRegion(rect1, ColoredText.Colorize("GenepackUnusableGenebankUnpowered".Translate(), ColorLibrary.RedReadable));
            }
            if (Mouse.IsOver(rect1))
                Widgets.DrawHighlight(rect1);
            if (Event.current?.type == null && Mouse.IsOver(rect1) && Event.current.button == 1)
            {
                List<FloatMenuOption> floatMenuOptionList = new List<FloatMenuOption>();
                TaggedString label = "EjectGenepackFromGeneBank".Translate();
                var option = new FloatMenuOption(label, (() =>
                {
                    CompGenepackContainer geneBankHoldingPack = this.geneSeparator.GetGeneBankHoldingPack(genepack);
                    if (geneBankHoldingPack == null)
                        return;
                    ThingWithComps parent = geneBankHoldingPack.parent;
                    if ((geneBankHoldingPack.innerContainer).TryDrop(genepack, parent?.def?.hasInteractionCell != null ? parent.InteractionCell : parent.Position, parent.Map, ThingPlaceMode.Near, 1, out Thing thing, null, null))
                    {
                        if (this.selectedGenepacks.Contains(genepack))
                            this.selectedGenepacks.Remove(genepack);
                        this.tmpGenes.Clear();
                        this.libraryGenepacks.Clear();
                        this.unpoweredGenepacks.Clear();
                        this.matchingGenepacks.Clear();
                        this.libraryGenepacks.AddRange(this.geneSeparator.GetGenepacks(true, true));
                        this.unpoweredGenepacks.AddRange(this.geneSeparator.GetGenepacks(false, true));
                        GeneUtility.SortGenepacks(this.libraryGenepacks);
                        GeneUtility.SortGenepacks(this.unpoweredGenepacks);
                        this.OnGenesChanged();
                    }
                }), MenuOptionPriority.Default, null, null, 0.0f, null, null, true, 0);

                floatMenuOptionList.Add(option);
                Find.WindowStack.Add(new FloatMenu(floatMenuOptionList));
            }
            else if (Widgets.ButtonInvisible(rect1, true))
                result = true;
            curX = Mathf.Max(curX, rect1.xMax + 14f);
            return result;
        }
        #endregion

        private static string GroupInfo(GeneLeftChosenGroup group) =>
            group == null
            ? null
            : ColoredText.Colorize(
                "GeneOneActive".Translate()
                    + ":\n  - "
                    + group.leftChosen.LabelCap
                    + " ("
                    + "Active".Translate()
                    + ")"
                    + "\n"
                    + GenText.ToLineList(group.overriddenGenes.Select(x =>
                        ColoredText.Colorize(x.label
                            + " ("
                            + "Suppressed".Translate()
                            + ")",
                            ColorLibrary.RedReadable)
                    ), "  - ", true),
            ColoredText.TipSectionTitleColor);
    }
}
