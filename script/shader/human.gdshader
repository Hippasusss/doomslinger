shader_type canvas_item;

void fragment() {
	// Sample the sprite's texture
	COLOR = texture(TEXTURE, UV);
}

void light() {
	// LIGHT_COLOR: The light's color property.
	// LIGHT_ENERGY: The light's energy/intensity property.
	// ATENUATION: The light's falloff and shadow masking.
	// COLOR: The sprite's texture color (from the fragment shader).
	
	LIGHT = (LIGHT_COLOR) * LIGHT_ENERGY * COLOR;
}
