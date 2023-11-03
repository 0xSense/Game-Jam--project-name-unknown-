using Godot;
using System;

public partial class Area2D : Godot.Area2D
{
	private Sprite2D _on;
	private Sprite2D _off;
	private bool _state;

    public override void _Ready()
    {
        _off = GetNode<Sprite2D>("../Off");
		_on = GetNode<Sprite2D>("../On");
		_state = true;
		_on.Visible = true;
    }

    public void _on_body_entered(Node2D body)
	{
		GD.Print("Body entered the collision area!");
		
		switch(_state){
			case true:
				_off.Visible = true;
				_on.Visible = false;
				_state = false;
				break;
			case false:
				_off.Visible = false;
				_on.Visible = true;
				_state = true;
				break;
		}

	}
}
