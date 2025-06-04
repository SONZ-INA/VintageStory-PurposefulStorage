namespace PurposefulStorage;

public interface IPurposefulStorageContainer {
    public ITreeAttribute VariantAttributes { get; set; }
    public bool OnInteract(IPlayer byPlayer, BlockSelection blockSel);
}
