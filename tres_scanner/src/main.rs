use serde_json::{Map, Value};
use std::fs;
use clap::Parser;

use walkdir::WalkDir;

const OUTPUT_FILE: &str = "../Client/config/assets/registry.json";

#[derive(Parser, Debug)]
struct Args {
    /// Root used to generate res:// paths
    #[arg(short, long, default_value="../Client")]
    root: String,

    /// Folder to scan for .tres files
    #[arg(short, long, default_value="../Client/assets/game/weapon")]
    scan: String,

    /// JSON key to write/update
    #[arg(short, long, default_value="weapons")]
    key: String,
}

fn to_res_path(path: &str, root: &str) -> Option<String> {
    let root = std::path::Path::new(root).canonicalize().ok()?;
    let path = std::path::Path::new(path).canonicalize().ok()?;

    let stripped = path.strip_prefix(&root).ok()?;

    Some(format!(
        "res://{}",
        stripped.display().to_string().replace('\\', "/")
    ))
}

fn scan_tres(scan_dir: &str, root: &str) -> Vec<String> {
    let mut out = Vec::new();

    for entry in WalkDir::new(scan_dir)
        .follow_links(true)
        .into_iter()
        .filter_map(Result::ok)
    {
        let path = entry.path();

        if path.is_file()
            && path.extension().and_then(|e| e.to_str()) == Some("tres")
        {
            if let Some(res_path) = to_res_path(&path.to_string_lossy(), root) {
                out.push(res_path);
            }
        }
    }

    out
}

fn load_json(path: &str) -> Value {
    match fs::read_to_string(path) {
        Ok(content) => serde_json::from_str(&content).unwrap_or(Value::Object(Map::new())),
        Err(_) => Value::Object(Map::new()),
    }
}

fn main() {
    let args = Args::parse();

    let files = scan_tres(&args.scan, &args.root);

    let mut json = load_json(OUTPUT_FILE);

    let obj = json.as_object_mut().expect("JSON root must be object");

    obj.insert(
        args.key,
        Value::Array(files.into_iter().map(Value::String).collect()),
    );

    fs::write(
        OUTPUT_FILE,
        serde_json::to_string_pretty(&json).unwrap(),
    )
    .expect("Failed to write JSON");

    println!("Updated {}", OUTPUT_FILE);
}