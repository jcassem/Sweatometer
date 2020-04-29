import React, { Component } from 'react';

export class EmojiResultTable extends Component {

    static searchWordName = "Search Word";

    constructor(state) {
        super(state);
        this.state = {
            searchWord: 'shutter island',
            loading: false,
            hasRun: false,
            statusCode: 200,
            errorMessage: '',
            emojis: []
        };

        this.poulateEmojiResultTable = this.poulateEmojiResultTable.bind(this);
        this.handleChange = this.handleChange.bind(this);
    }

    handleChange(event) {
        this.setState({
            searchWord: event.target.value
        });
    }

    async poulateEmojiResultTable(e) {
        e.preventDefault()

        this.setState({
            loading: true,
            hasRun: true,
            emojis: []
        });

        try {
            const response = await fetch('api/wordfinder/find/emoji/set/' + this.state.searchWord)
            const status = await response.status;
            const statusText = await response.statusText;

            if (status !== 200) {
                this.setState({
                    statusCode: status,
                    errorMessage: statusText
                });
            }
            else {
                const data = await response.json();

                this.setState({
                    emojis: data,
                    loading: false,
                    statusCode: status,
                    errorMessage: ''
                });
            }

        } catch (error) {
            this.setState({
                errorMessage: error.message
            });
        }

    }

    static renderEmojiSetTable(emojisSet) {

        if (emojisSet == null || emojisSet.length === 0) {
           return (<h1 class="no-merge-result">No results found</h1>);
        }

        var combinedFirstResult = "";
        Object.keys(emojisSet).map((key, i) => combinedFirstResult += emojisSet[key][0].icon + " ");

        return (
            <div class="container">
                <div class="row">
                    <div class="card emoji-set-card">
                        <div class="card-body">
                            <h1>{combinedFirstResult}</h1>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <table className='table table-striped' aria-labelledby="tabelLabel">
                        <thead>
                            <tr>
                                <th>Word</th>
                                <th>Emojis</th>
                            </tr>
                        </thead>
                        <tbody>
                            {Object.keys(emojisSet).map((key, i) =>
                                <tr key={i}>
                                    <td>{key}</td>
                                    <td>{EmojiResultTable.renderEmojiTable(emojisSet[key])}</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }

    static renderEmojiTable(emojis) {

        if (emojis == null || emojis.length === 0) {
           return (<h1 class="no-merge-result">No results found</h1>);
        }
        else if (emojis.length == 1) {
           return (
               <div class="single-merge-result">
                   <h1>{emojis[0].icon}</h1>
                   <p>{emojis[0].description}</p>
               </div>
           );
        }

        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Emoji</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>
                    {emojis.map(emoji =>
                        <tr key={emoji.description}>
                            <td>{emoji.icon}</td>
                            <td>{emoji.description}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let result = '';
        if (this.state.hasRun) {
            if (this.state.statusCode != 200 || this.state.errorMessage.length !== 0) {
                result =
                    <div class="alert alert-danger" role="alert">
                        <h2>{this.state.statusCode}</h2><p>{this.state.errorMessage}</p>
                    </div>;
            }
            else {
                if (this.state.loading) {
                    result = 'loading'
                }
                else {
                    result = EmojiResultTable.renderEmojiSetTable(this.state.emojis)
                }
            }
        }


    return (
        <div>
            <form onSubmit={this.poulateEmojiResultTable}>
                <div class="form-row">
                    <div class="col-md-6 mb-6">
                        <label for="searchWord">{EmojiResultTable.searchWordName}:</label>
                        <input class="form-control" type="text" id="searchWord" 
                            name="searchWord" value={this.state.searchWord} onChange={this.handleChange} />
                    </div>
                </div>

                <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
            </form>
            <div>{result}</div>
        </div>
    );
  }
}
