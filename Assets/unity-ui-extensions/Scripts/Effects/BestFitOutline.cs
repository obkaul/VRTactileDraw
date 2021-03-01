/// Credit Melang
/// Sourced from - http://forum.unity3d.com/members/melang.593409/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Effects
{
    [AddComponentMenu("UI/Effects/Extensions/BestFit Outline")]
	public class BestFitOutline : Shadow
	{
		//
		// Constructors
		//
		protected BestFitOutline ()
		{
		}
		
		//
		// Methods
		//
		public override void ModifyMesh (Mesh mesh)
		{
			if (!this.IsActive ())
			{
				return;
			}

            var verts = new List<UIVertex>();
            using (var helper = new VertexHelper(mesh))
            {
                helper.GetUIVertexStream(verts);
            }

            var foundtext = GetComponent<Text>();

			var best_fit_adjustment = 1f;

			if (foundtext && foundtext.resizeTextForBestFit)  
			{
				best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / (foundtext.resizeTextMaxSize-1); //max size seems to be exclusive 
			}
			
			var start = 0;
			var count = verts.Count;
			base.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, base.effectDistance.x*best_fit_adjustment, base.effectDistance.y*best_fit_adjustment);
			start = count;
			count = verts.Count;
			base.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, base.effectDistance.x*best_fit_adjustment, -base.effectDistance.y*best_fit_adjustment);
			start = count;
			count = verts.Count;
			base.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, -base.effectDistance.x*best_fit_adjustment, base.effectDistance.y*best_fit_adjustment);
			start = count;
			count = verts.Count;
			base.ApplyShadowZeroAlloc(verts, base.effectColor, start, verts.Count, -base.effectDistance.x*best_fit_adjustment, -base.effectDistance.y*best_fit_adjustment);

            using (var helper = new VertexHelper())
            {
                helper.AddUIVertexTriangleStream(verts);
                helper.FillMesh(mesh);
            }
        }
	}
}
