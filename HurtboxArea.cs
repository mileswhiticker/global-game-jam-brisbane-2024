using Godot;
using System;

public partial class HurtboxArea : Area2D
{
    private void _on_hurtbox_area_body_shape_entered(CollisionObject2D collisionObject, Node2D body, int bodyShapeIndex, int localShapeIndex)
    {
        // trigger the signal for the parent Player node
    }
}
