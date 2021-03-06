/***
 * Excerpted from "Practical Vim",
 * published by The Pragmatic Bookshelf.
 * Copyrights apply to this code. It may not be used to create training material, 
 * courses, books, articles, and the like. Contact us if you are in doubt.
 * We make no guarantees that this code is fit for any purpose. 
 * Visit http://www.pragmaticprogrammer.com/titles/dnvim for more book information.
***/
Markdown.dialects.Gruber = {
    lists: function() {
        // TODO: Regex für bestimmte Tiefen cachen.
        function regex_for_depth(depth) { /* implementation */ }
    },
    "`": function inlineCode( text ) {
        var m = text.match( /(`+)(([\s\S]*?)\1)/ );
        if ( m && m[2] )
            return [ m[1].length + m[2].length ];
        else {
            // TODO: Kein passender Endcode gefunden - Warnung melden!
            return [ 1, "`" ];
        }
    }
}
