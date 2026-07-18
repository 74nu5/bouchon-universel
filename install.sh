#!/usr/bin/env sh
# Installe et lance Bouchon Universel (binaire autonome, sans .NET installé).
#   curl -fsSL https://raw.githubusercontent.com/74nu5/bouchon-universel/master/install.sh | sh
set -e

REPO="74nu5/bouchon-universel"
DIR="${BOUCHON_DIR:-$HOME/.bouchon-universel}"

case "$(uname -s)" in
  Linux)  RID="linux-x64" ;;
  Darwin) RID="osx-arm64" ;;
  *) echo "Système non supporté : $(uname -s). Utilisez Docker ou un binaire manuel." >&2; exit 1 ;;
esac

URL="https://github.com/$REPO/releases/latest/download/bouchon-universel-$RID.tar.gz"

mkdir -p "$DIR/data/files"
echo "Téléchargement de $URL ..."
curl -fsSL "$URL" -o "$DIR/bouchon.tar.gz"
tar -xzf "$DIR/bouchon.tar.gz" -C "$DIR"
chmod +x "$DIR/BouchonUniversel"
rm -f "$DIR/bouchon.tar.gz"

echo "Lancement de Bouchon Universel sur http://localhost:8080 (Ctrl+C pour arrêter)"
ASPNETCORE_URLS="http://+:8080" \
ASPNETCORE_ENVIRONMENT="Production" \
ConnectionStrings__BDDConnection="$DIR/data/BouchonUniversel.db" \
Bouchon__CheminFichiers="$DIR/data/files" \
  "$DIR/BouchonUniversel"
