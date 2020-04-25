import React, { Component } from 'react';

export class EmojiResultTable extends Component {

    static searchWordName = "Search Word";

    constructor(state) {
        super(state);
        this.state = {
            searchWord: 'smile',
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
            const response = await fetch('api/wordfinder/find/emoji/' + this.state.searchWord)
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

    static renderEmojiTable(emojis) {

        if (emojis == null || emojis.length === 0) {
           return (<h1 class="no-merge-result">No results found</h1>);
        }
        else if (emojis.length == 1) {
           return (
               <div class="  ngle-merge-result">
                   <h1>{emojis[0].Icon}</h1>
                   <p>{emojis[0].Description}</p>
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
                    result = EmojiResultTable.renderEmojiTable(this.state.emojis)
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
