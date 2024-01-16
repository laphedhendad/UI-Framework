using UnityEngine;

namespace Laphed.MVP.MonoViews
{
    public abstract class MonoSpriteView: MonoView<Sprite>
    {
        protected abstract Sprite Sprite { set; }
        public override void UpdateView(Sprite value) => Sprite = value;
    }
}