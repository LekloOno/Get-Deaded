@tool
extends EditorPlugin


func _enter_tree():
    # Initialization of the plugin goes here
    # Add the new type with a name, a parent type, a script and an icon
    var icon = EditorInterface.get_base_control().get_theme_icon("CharacterBody3D", "EditorIcons")
    add_custom_type("PI_Walk", "Node", preload("PI_Walk.cs"), icon)
    pass


func _exit_tree():
    # Clean-up of the plugin goes here
    # Always remember to remove_at it from the engine when deactivated
    remove_custom_type("PI_Walk")
