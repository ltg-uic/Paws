//****************************************************************************************************
//  static function DrawLine(rect : Rect) : void
//  static function DrawLine(rect : Rect, color : Color) : void
//  static function DrawLine(rect : Rect, width : float) : void
//  static function DrawLine(rect : Rect, color : Color, width : float) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, width : float) : void
//  static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color, width : float) : void
//  
//  Draws a GUI line on the screen.
//  
//  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
//  This function works by drawing a 1x1 texture filled with a color, which is then scaled
//   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
//****************************************************************************************************

static var lineTex : Texture2D;

static function DrawLine(rect : Rect) { DrawLine(rect, GUI.contentColor, 1.0); }
static function DrawLine(rect : Rect, color : Color) { DrawLine(rect, color, 1.0); }
static function DrawLine(rect : Rect, width : float) { DrawLine(rect, GUI.contentColor, width); }
static function DrawLine(rect : Rect, color : Color, width : float) { DrawLine(Vector2(rect.x, rect.y), Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
static function DrawLine(pointA : Vector2, pointB : Vector2) { DrawLine(pointA, pointB, GUI.contentColor, 1.0); }
static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color) { DrawLine(pointA, pointB, color, 1.0); }
static function DrawLine(pointA : Vector2, pointB : Vector2, width : float) { DrawLine(pointA, pointB, GUI.contentColor, width); }
static function DrawLine(pointA : Vector2, pointB : Vector2, color : Color, width : float) {
    // Save the current GUI matrix, since we're going to make changes to it.
    var matrix = GUI.matrix;
    
    // Generate a single pixel texture if it doesn't exist
    if (!lineTex) {
        lineTex = Texture2D(1, 1);
        lineTex.SetPixel(0, 0, Color.white);
        lineTex.Apply();
    }
    
    // Store current GUI color, so we can switch it back later,
    // and set the GUI color to the color parameter
    var savedColor = GUI.color;
    GUI.color = color;

    // Determine the angle of the line.
    var angle = Vector3.Angle(pointB-pointA, Vector2.right);
    
    // Vector3.Angle always returns a positive number.
    // If pointB is above pointA, then angle needs to be negative.
    if (pointA.y > pointB.y) { angle = -angle; }
    
    // Use ScaleAroundPivot to adjust the size of the line.
    // We could do this when we draw the texture, but by scaling it here we can use
    //  non-integer values for the width and length (such as sub 1 pixel widths).
    // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
    //  is centered on the origin at pointA.
    GUIUtility.ScaleAroundPivot(Vector2((pointB-pointA).magnitude, width), Vector2(pointA.x, pointA.y + 0.5));
    
    // Set the rotation for the line.
    //  The angle was calculated with pointA as the origin.
    GUIUtility.RotateAroundPivot(angle, pointA);
    
    // Finally, draw the actual line.
    // We're really only drawing a 1x1 texture from pointA.
    // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
    //  render with the proper width, length, and angle.
    GUI.DrawTexture(Rect(pointA.x, pointA.y, 1, 1), lineTex);
    
    // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
    GUI.matrix = matrix;
    GUI.color = savedColor;
}
 